const { MongoClient } = require('mongodb');

async function fix() {
  const uri = "mongodb://localhost:27017";
  const client = new MongoClient(uri);
  try {
    await client.connect();
    const db = client.db('AMS');
    
    // First remove the old lowercase fields that didn't map
    await db.collection('StudentTbl').updateMany({}, {
      $unset: {
        dob: "", doa: "", address: "", parentName: "", parentMobile: "", parentEmail: ""
      }
    });

    // Now set the correctly cased fields with Date objects
    await db.collection('StudentTbl').updateMany({}, {
      $set: {
        DOB: new Date("2000-01-01T00:00:00Z"),
        DOA: new Date("2020-08-01T00:00:00Z"),
        Address: "123 Main Street, New York, NY 10001",
        ParentName: "Richard Doe",
        ParentMobile: "9000000000",
        ParentEmail: "richard@example.com"
      }
    });

    console.log("Database perfectly fixed!");
  } finally {
    await client.close();
  }
}
fix().catch(console.error);
