DROP TABLE IF EXISTS ITEM_HIS ;
DROP TABLE IF EXISTS ITEM;
DROP TABLE IF EXISTS PAYMENT;
DROP TABLE IF EXISTS ITEM_PAYMENT;
DROP TABLE IF EXISTS CATEGORY;
DROP TABLE IF EXISTS CONDITIONS;
DROP TABLE IF EXISTS SELLINGSTATE;
DROP TABLE IF EXISTS SELLER;

CREATE TABLE ITEM_HIS (
    itemid            VARCHAR(40) NOT NULL PRIMARY KEY,
    update_mark		  DateTime
);

CREATE TABLE ITEM (
    itemid            VARCHAR(40) NOT NULL PRIMARY KEY,
    category_id       VARCHAR(40) NOT NULL,
    condition_id      VARCHAR(40) NOT NULL,
    sellingstate_id   INTEGER NOT NULL,
    title             VARCHAR(4000) NOT NULL,
    price             DECIMAL(16, 2) NOT NULL,
    currency_name     VARCHAR(40) NOT NULL,
    bidcount          VARCHAR(40) NOT NULL,
    listingtype       VARCHAR(40) NOT NULL,
    seller_name       VARCHAR(40) NOT NULL,
    end_time          DateTime
);

CREATE TABLE PAYMENT (
    id         INTEGER NOT NULL IDENTITY PRIMARY KEY,
    paymentname   VARCHAR(40)
);

CREATE TABLE ITEM_PAYMENT (
    itemid      VARCHAR(40)  NOT NULL,
    paymentid   INTEGER  NOT NULL
);

CREATE TABLE SELLER (
    name                      VARCHAR(40) PRIMARY KEY,
    feedbackscore             INTEGER NOT NULL,
    positivefeedbackpercent   INTEGER NOT NULL,
    feedbackratingstar        VARCHAR(40) NOT NULL
);


CREATE TABLE CATEGORY (
    id          VARCHAR(40) NOT NULL PRIMARY KEY,
    categoryname   VARCHAR(40) NOT NULL
);

CREATE TABLE CONDITIONS (
    id          VARCHAR(40) NOT NULL PRIMARY KEY,
    conditionname   VARCHAR(40) NOT NULL
);

CREATE TABLE SELLINGSTATE (
    id                 INTEGER NOT NULL IDENTITY PRIMARY KEY,
    sellingstatename   VARCHAR(40) NOT NULL
);

ALTER TABLE ITEM
    ADD CONSTRAINT item_seller_fk FOREIGN KEY ( seller_name )
        REFERENCES SELLER ( name );

ALTER TABLE ITEM
    ADD CONSTRAINT item_category_fk FOREIGN KEY ( category_id )
        REFERENCES CATEGORY ( id );

ALTER TABLE ITEM
    ADD CONSTRAINT item_condition_fk FOREIGN KEY ( condition_id )
        REFERENCES CONDITIONS ( id );

ALTER TABLE ITEM
    ADD CONSTRAINT item_sellingstate_fk FOREIGN KEY ( sellingstate_id )
        REFERENCES SELLINGSTATE ( id );


ALTER TABLE ITEM_PAYMENT
	ADD 
CONSTRAINT item_payment_pk PRIMARY KEY 
( itemid,
	paymentid
) 