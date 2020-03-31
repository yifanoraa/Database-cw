# -*- coding: utf-8 -*-
import pymysql

def get_db_conn():
    return pymysql.connect(host='db', port=3306, user='ebay', passwd='ebay', db='ebay_info', charset='utf8')

