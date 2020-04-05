# -*- coding: utf-8 -*-

import pymysql
from datetime import datetime
from eBay_base import get_db_conn

def db_clear_data(conn):
    with conn :
        conn.autocommit = False
        cursor = conn.cursor()
        cursor.execute("DELETE FROM ITEM_PAYMENT")
        cursor.execute("DELETE FROM ITEM")
        cursor.execute("DELETE FROM PAYMENT")
        cursor.execute("DELETE FROM CATEGORY")
        cursor.execute("DELETE FROM CONDITIONS")
        cursor.execute("DELETE FROM SELLINGSTATE")
        cursor.execute("DELETE FROM SELLER")
        conn.commit()
        
def get_l1_category_list(conn):
    result = None
    
    with conn :
        cursor = conn.cursor(pymysql.cursors.DictCursor)
        cursor.execute("SELECT * FROM CATEGORY WHERE LENGTH(ID) = 3")
        result = cursor.fetchall()
        
    return result

def get_l2_category_list(conn, l1_category):
    result = None
    
    with conn :
        cursor = conn.cursor(pymysql.cursors.DictCursor)
        
        if l1_category is None :
            cursor.execute("""
                SELECT * FROM CATEGORY WHERE 1=1
                ORDER BY CATEGORYNAME ASC
                """)
        else :
            cursor.execute("""
                SELECT * FROM CATEGORY WHERE 1=1 
                AND LEFT(ID, 3) = %s
                ORDER BY CATEGORYNAME ASC
                """, l1_category)
            
        result = cursor.fetchall()
        
    return result

def list_items(conn, category_id) :
    result = None
    
    with conn :
        cursor = conn.cursor(pymysql.cursors.DictCursor)
        
        if category_id is None :
            cursor.execute("""
                SELECT *, (SELECT CATEGORYNAME FROM CATEGORY WHERE ID = CATEGORY_ID) category_name 
                FROM ITEM 
                WHERE 1=1
                """)
        else :
            cursor.execute("""
                SELECT *, (SELECT CATEGORYNAME FROM CATEGORY WHERE ID = CATEGORY_ID) category_name 
                FROM ITEM 
                WHERE 1=1
                AND CATEGORY_ID = %s
                """, category_id)
            
        result = cursor.fetchall()
        
    return result
    
def list_items_chart(conn, category_id) :
    result = None
    
    with conn :
        cursor = conn.cursor(pymysql.cursors.DictCursor)
        
        if category_id is None :
            cursor.execute("""
                SELECT MIN(PRICE) min_price
                    , MAX(price) max_price
                    , AVG(price) avg_price
                    , COUNT(1) cnt
                    , DATE(end_time) end_date
                FROM ITEM 
                WHERE 1=1
                GROUP BY DATE(end_time)
                ORDER BY end_date
                """)
        else :
            cursor.execute("""
                SELECT MIN(PRICE) min_price
                    , MAX(price) max_price
                    , AVG(price) avg_price
                    , COUNT(1) cnt
                    , DATE(end_time) end_date
                FROM ITEM 
                WHERE 1=1
                AND CATEGORY_ID = %s
                GROUP BY DATE(end_time)
                ORDER BY end_date
                """, category_id)
            
        result = cursor.fetchall()
    
    chart_data = {'xAxis': [], 'series': []}
    
    chart_data['xAxis'].append({'type': 'category', 'data': [datetime.strftime(r['end_date'], '%Y%m%d') for r in result]})
        
    chart_data['series'].append({'name': 'min_price', 'type': 'line', 'data': [str(r['min_price']) for r in result]})
    chart_data['series'].append({'name': 'max_price', 'type': 'line', 'data': [str(r['max_price']) for r in result]})
    chart_data['series'].append({'name': 'avg_price', 'type': 'line', 'data': [str(r['avg_price']) for r in result]})
    chart_data['series'].append({'name': 'cnt', 'type': 'bar', 'data': [r['cnt'] for r in result]})
        
    return chart_data

if __name__ == "__main__":
    conn = get_db_conn()
    
#    db_clear_data(conn)
    
#    result = get_l1_category_list(conn)
#    print(result)
#    
#    result = get_l2_category_list(conn, None)
#    print(result)
#    
#    result = list_items(conn, None)
#    print(result)
    
    result = list_items_chart(conn, None)
    print(result)
    
    pass

