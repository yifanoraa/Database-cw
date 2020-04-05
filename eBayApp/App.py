# coding: utf-8
import logging
from flask import Flask, render_template, redirect, url_for
from flask import request

#from waitress import serve

logging.basicConfig(level=logging.DEBUG,
    format='%(asctime)s %(filename)s[line:%(lineno)d] %(levelname)s %(message)s',
    datefmt='%Y-%m-%d %H:%M:%S')

logger = logging.getLogger()

app = Flask(__name__)
app.config['SECRET_KEY'] = "random string"
app.config['SQLALCHEMY_TRACK_MODIFICATIONS'] = True

import json
from eBay_base import get_db_conn
from eBay_parser import get_info
from eBay_db import get_l2_category_list, list_items, db_clear_data, list_items_chart

@app.route("/")
def index():
    return render_template('index.html')

@app.route("/search", methods=["post"])
def search():
    keywords = request.form.get("keywords")
    clear_data = request.form.get("clear_data")

    logger.debug("searching keywords {}, clear old data {} ...".format(keywords, clear_data))

    conn = get_db_conn()
    
    if clear_data == "Y":
        db_clear_data(conn)
    error_message = get_info(conn, keywords)
    
    conn.close()
    
    if error_message == "":
        return redirect(url_for('list'))
    else :
        return render_template('error.html', error_message=error_message)

@app.route("/list", methods=["post", "get"])
def list():
    category_id = request.form.get("category_id")
    logger.debug("searching by category_id {} ...".format(category_id))
    
    show_detail = request.form.get("show_detail")
    
    conn = get_db_conn()
    category_list = get_l2_category_list(conn, None)
    for category in category_list :
        category["selected"] = category["id"] == category_id
    
    item_list = list_items(conn, category_id)
    conn.close()
    
    return render_template('list.html', category_list=category_list, item_list=item_list, show_detail=show_detail)

@app.route("/list_chart", methods=["post", "get"])
def list_chart():
    category_id = request.form.get("category_id")
    logger.debug("list_chart by category_id {} ...".format(category_id))
    
    conn = get_db_conn()
    chart_data = list_items_chart(conn, category_id)
    conn.close()
    
    return json.dumps(chart_data)

@app.route("/list_detail", methods=["post", "get"])
def list_detail():
    category_id = request.form.get("category_id")
    logger.debug("searching by category_id {} ...".format(category_id))
       
    conn = get_db_conn()
    category_list = get_l2_category_list(conn, None)
    for category in category_list :
        category["selected"] = category["id"] == category_id
    
    item_list = list_items(conn, category_id)
    conn.close()
    
    return render_template('list_detail.html', category_list=category_list, item_list=item_list)

if __name__ == "__main__" :
    app.run(host="0.0.0.0", debug=True) 
#    serve(app=app, host="0.0.0.0", port=5000)
