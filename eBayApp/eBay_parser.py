# -*- coding: utf-8 -*-
import sys
sys.path.insert(0, '/opt/www/ebaysdk-python/')

import ebaysdk
from ebaysdk.finding import Connection as finding
from ebaysdk.exception import ConnectionError

from eBay_base import get_db_conn

def func_test(conn):
    with conn :
        conn.autocommit = False
        cursor = conn.cursor()
        cursor.execute("DELETE FROM ITEM_PAYMENT")
        cursor.execute("DELETE FROM ITEM")
        conn.commit()

def handle_item_category(conn, it):
    with conn:
        cursor = conn.cursor()
        cursor.execute("SELECT 1 FROM CATEGORY WHERE ID = %s", it.primaryCategory.categoryId)
        if cursor.fetchone() is None :
            cursor.execute("INSERT INTO CATEGORY VALUES(%s, %s)", (it.primaryCategory.categoryId, it.primaryCategory.categoryName))            
            conn.commit()
    pass

def handle_item_condition(conn, it):
    with conn:
        cursor = conn.cursor()
        cursor.execute("SELECT 1 FROM CONDITIONS WHERE ID = %s", it.condition.conditionId)
        if cursor.fetchone() is None :
            cursor.execute("INSERT INTO CONDITIONS VALUES(%s, %s)", (it.condition.conditionId, it.condition.conditionDisplayName))            
            conn.commit()
    pass

def handle_item_seller(conn, it):
    with conn:
        cursor = conn.cursor()
        cursor.execute("SELECT 1 FROM SELLER WHERE NAME = %s", it.sellerInfo.sellerUserName)
        if cursor.fetchone() is None :
            cursor.execute("INSERT INTO SELLER VALUES(%s, %s, %s, %s)", (it.sellerInfo.sellerUserName
                           , it.sellerInfo.feedbackScore
                           , it.sellerInfo.positiveFeedbackPercent
                           , it.sellerInfo.feedbackRatingStar))            
            conn.commit()
    pass
    
def handle_item_payment(conn, it):
    ret_value = None
    payMethods = []
    paymentMethod = it.paymentMethod
    
    if type(paymentMethod) == str:
        payMethods.append(paymentMethod)
    else :
        payMethods = paymentMethod
    
    with conn:
        cursor = conn.cursor()
        cursor.execute("DELETE FROM ITEM_PAYMENT WHERE ITEMID = %s", it.itemId)
        
        for method in payMethods :
            cursor.execute("SELECT ID FROM PAYMENT WHERE PAYMENTNAME = %s", method)
            row = cursor.fetchone()
            if row is None :
                cursor.execute("INSERT INTO PAYMENT(PAYMENTNAME) VALUES(%s)", (method))            
                ret_value = cursor.lastrowid
            else:
                ret_value = row[0]
                
            cursor.execute("INSERT INTO ITEM_PAYMENT(ITEMID, PAYMENTID) VALUES(%s, %s)", (it.itemId, ret_value))
            pass
        conn.commit()
                    
    return ret_value

def handle_item_sellingstate(conn, it):
    ret_value = None
    
    with conn:
        cursor = conn.cursor()
        cursor.execute("SELECT ID FROM SELLINGSTATE WHERE sellingstatename = %s", it.sellingStatus.sellingState)
        row = cursor.fetchone()
        if row is None :
            cursor.execute("INSERT INTO SELLINGSTATE(sellingstatename) VALUES(%s)", (it.sellingStatus.sellingState))            
            conn.commit()
            ret_value = cursor.lastrowid
        else:
            ret_value = row[0]
                    
    return ret_value

def handle_item(conn, it):
    # Skip item without conditon or paymentMethod
    try :
        it.condition
        it.paymentMethod
    except :
        return

#    print("--------------------")
#    print(it)
    print("====================")
    print(it.itemId)
    print(it.primaryCategory.categoryName)
    print(it.title)
#    print(it.condition.conditionDisplayName)
#    print(it.viewItemURL)
#    
#    print(it.sellingStatus.currentPrice)
#    print(it.sellingStatus.bidCount)
#    print(it.sellingStatus.sellingState)
#    print(it.listingInfo.endTime)
#    
#    print(it.sellerInfo.sellerUserName)
#    print(it.sellerInfo.feedbackScore)
#    print(it.sellerInfo.positiveFeedbackPercent)
#    print(it.sellerInfo.feedbackRatingStar)
    
    handle_item_category(conn, it)
    handle_item_condition(conn, it)
    handle_item_seller(conn, it)
    sellingstate_id = handle_item_sellingstate(conn, it)
    
    with conn :
        cursor = conn.cursor()
        cursor.execute("DELETE FROM ITEM WHERE ITEMID = %s", it.itemId)
        cursor.execute("""
            INSERT INTO ITEM(ITEMID
                             , TITLE
                             , CATEGORY_ID
                             , CONDITION_ID
                             , SELLINGSTATE_ID
                             , SELLER_NAME
                             , BIDCOUNT
                             , listingtype
                             , price
                             , currency_name
                             , end_time
                        ) VALUES(%s
                             , %s
                             , %s
                             , %s
                             , %s
                             , %s
                             , %s
                             , %s
                             , %s
                             , %s
                             , %s)
            """
                       , (it.itemId
                          , it.title
                          , it.primaryCategory.categoryId 
                          , it.condition.conditionId
                          , sellingstate_id
                          , it.sellerInfo.sellerUserName
                          , it.sellingStatus.bidCount
                          , it.listingInfo.listingType
                          , it.sellingStatus.currentPrice.value
                          , it.sellingStatus.currentPrice._currencyId
                          , it.listingInfo.endTime))            
        conn.commit()
        pass
    
    handle_item_payment(conn, it)

def has_items(resp):
    if resp is None :
        return False
    
    reply = resp.reply
    if reply is None :
        return False
    
    searchResult = reply.searchResult
    if searchResult is None :
        return False
    
    if searchResult._count == '0':
        return False
    else :
        return True

def get_info(conn, keywords):
    error_message = ""
    
    try:
        opts_appid = 'ZimingWu-comp0022-PRD-b69ec6afc-7ff00c17'
        opts_yaml = './ebaysdk-python/ebay.yaml'
        opts_domain = 'svcs.ebay.com'

        api = finding(debug=False, appid=opts_appid, domain=opts_domain,
                      config_file=opts_yaml, warnings=True)

        api_request = {
            'keywords': keywords,
            'itemFilter': [
                {'name': 'ListingType',
                 'value': 'AuctionWithBIN'},
            ],
            'outputSelector': ['SellerInfo'],
        }

        response = api.execute('findCompletedItems', api_request)
#        response = api.execute('findItemsAdvanced', api_request)

        item_count = 0
        while has_items(response) :
            reply = response.reply
            searchResult = reply.searchResult
            
            if searchResult._count == '0' :
                print('Empty result')
            else :
                print('Get {} result, and list Auction items'.format(searchResult._count))
                for it in searchResult.item :
                    item_count = item_count + 1
                    try:
                        handle_item(conn, it)
                    except Exception as ex:
                        err = str(ex)
                        if err.startswith('(1366,') :
                            print('-------------> {}'.format(err))
                            pass
                        else :
                            raise ex
            
            if item_count >= 500 :
                break
            
            # Loop to next page
            response = api.next_page()
                    
        print('Done')
    except Exception as e:
        print(e)
        error_message = str(e)
        
    return error_message    # Read time out or other error
        
if __name__ == "__main__":
    get_info(get_db_conn(), 'Apple')

