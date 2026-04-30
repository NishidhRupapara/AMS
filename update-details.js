const { MongoClient } = require('mongodb');

async function seed() {
  const uri = "mongodb://localhost:27017";
  const client = new MongoClient(uri);

  try {
    await client.connect();
    const db = client.db('AMS');

    // Update Students with all details
    await db.collection('StudentTbl').updateMany({}, {
      $set: {
        dob: "2000-01-01",
        doa: "2020-08-01",
        address: "123 Main Street, New York, NY 10001",
        parentName: "Richard Doe",
        parentMobile: "9000000000",
        parentEmail: "richard@example.com"
      }
    });

    // Also update faculties just in case
    await db.collection('faculties').updateMany({}, {
      $set: {
        address: "456 University Avenue",
        education: "PhD in Computer Science"
      }
    });

    console.log("Database updated with ALL details!");
  } finally {
    await client.close();
  }
}

seed().catch(console.error);
