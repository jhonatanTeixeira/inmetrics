from dataclasses import dataclass
import pymongo


@dataclass
class MongoContext:
    url: str
    database: str
    collection: str


def get_database(context: MongoContext):
    client = pymongo.MongoClient(context.url)
    
    return client[context.database]


def get_collection(context: MongoContext):
    return get_database(context)[context.collection]


def save(context: MongoContext, document):
    return get_collection(context).insert_one(document)


def update(context: MongoContext, filter, data):
    return get_collection(context).update_one(filter, {"$set": data})
    

def find_one(context: MongoContext, filter):
    return get_collection(context).find_one(filter)


def add_index(context: MongoContext, index_names: list[str], unique=False):
    get_collection(context).create_index([(index, pymongo.ASCENDING) for index in index_names], unique=unique)
