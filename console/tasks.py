from invoke import task
import mongo
import os
import bcrypt

users_mongo_url = os.getenv('USERS_MONGO_URL') or 'mongodb://localhost:27017'
users_database = os.getenv('USERS_DATABASE') or 'users'
users_collection = os.getenv('USERS_COLLECTION') or 'users'

transactions_mongo_url = os.getenv('TRANSACTIONS_MONGO_URL') or 'mongodb://localhost:27017'
transactions_database = os.getenv('TRANSACTIONS_DATABASE') or 'transactions'
transactions_collection = os.getenv('TRANSACTIONS_COLLECTION') or 'transactions'

daily_cash_flow_mongo_url = os.getenv('DAILY_CASH_FLOW_MONGO_URL') or 'mongodb://localhost:27017'
daily_cash_flow_database = os.getenv('DAILY_CASH_FLOW_DATABASE') or 'daily_cash_flow'
daily_cash_flow_collection = os.getenv('DAILY_CASH_FLOW_COLLECTION') or 'daily_cash_flow'


@task
def create_user(c, name, email, password):
    context = mongo.MongoContext(users_mongo_url, users_database, users_collection)

    user = {
        "Name": name,
        "Email": email,
        "Password": bcrypt.hashpw(password.encode('utf-8'), bcrypt.gensalt(11, b"2a")).decode('utf-8'),
    }

    print(mongo.save(context, user).inserted_id)


@task
def update_user_password(c, email, password):
    context = mongo.MongoContext(users_mongo_url, users_database, users_collection)

    data = {"Password": bcrypt.hashpw(password.encode('utf-8'), bcrypt.gensalt(11, b"2a")).decode('utf-8')}

    print(mongo.update(context, {"Email": email}, data).modified_count)


@task
def create_indexes(c):
    users_context = mongo.MongoContext(users_mongo_url, users_database, users_collection)
    transactions_context = mongo.MongoContext(transactions_mongo_url, transactions_database, transactions_collection)
    dcf_context = mongo.MongoContext(daily_cash_flow_mongo_url, daily_cash_flow_database, daily_cash_flow_collection)

    mongo.add_index(users_context, ['Email'], unique=True)
    mongo.add_index(transactions_context, ['UserId'])
    mongo.add_index(dcf_context, ['UserId'])