1. docker_flask是docker环境，ER_Drawing是ER图，ebay_info.sql是建库语句。

2. How to run
    2.1 cd docker_flask
    2.2 docker-compose up -build

    2.3 access phpmyadmin from http://localhost
        host: db
        user name: ebay
        password: ebay
    2.4 create ebay database by using ebay_info.sql

    2.5 access app from http://localhost:5000

3. By default, only query 100 item from ebay, you can comment out line 222 in docker_flask/eBayApp/eBay_parser.py to load more items from eaby.

