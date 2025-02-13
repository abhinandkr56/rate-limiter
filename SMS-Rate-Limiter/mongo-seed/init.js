// Connect to the database
db = db.getSiblingDB("rateLimiter");

// Drop collections if they exist (optional, useful for re-runs)
db.accounts.drop();
db.businessNumbers.drop();
db.history.drop();

// Create collections
db.createCollection("accounts");
db.createCollection("businessNumbers");
db.createCollection("history");

// Insert one dummy Account
db.accounts.insertOne({
    "_id": ObjectId(),
    "AccountName": "TestAccount",
    "GlobalLimit": 1000
});

// Fetch the inserted AccountId
const account = db.accounts.findOne();
const accountId = account._id.toString(); // Convert ObjectId to string

// Insert multiple dummy BusinessNumbers linked to the Account
db.businessNumbers.insertMany([
    { "_id": ObjectId(), "AccountId": accountId, "PhoneNumber": "+1234567890", "PerSecondLimit": 10 },
    { "_id": ObjectId(), "AccountId": accountId, "PhoneNumber": "+1234567891", "PerSecondLimit": 15 },
    { "_id": ObjectId(), "AccountId": accountId, "PhoneNumber": "+1234567892", "PerSecondLimit": 20 },
    { "_id": ObjectId(), "AccountId": accountId, "PhoneNumber": "+1234567893", "PerSecondLimit": 25 },
    { "_id": ObjectId(), "AccountId": accountId, "PhoneNumber": "+1234567894", "PerSecondLimit": 30 }
]);

print("Database and collections created successfully!");
