const { MongoClient } = require('mongodb');

async function seed() {
  const uri = "mongodb://localhost:27017";
  const client = new MongoClient(uri);

  try {
    await client.connect();
    const db = client.db('AMS');

    // CORRECT COLLECTION NAME IS "Department" (Singular)
    try { await db.collection('Department').drop(); } catch (e) {}
    try { await db.collection('faculties').drop(); } catch (e) {}
    try { await db.collection('StudentTbl').drop(); } catch (e) {}
    try { await db.collection('counters').drop(); } catch (e) {}
    try { await db.collection('AttendanceRecords').drop(); } catch (e) {}

    // Seed Department
    await db.collection('Department').insertMany([
      { Did: 1, DepartmentName: "Computer Science", ImageUrl: "https://images.unsplash.com/photo-1517694712202-14dd9538aa97?w=500&q=80" },
      { Did: 2, DepartmentName: "Mechanical Engineering", ImageUrl: "https://images.unsplash.com/photo-1581092334651-ddf26d9a09d0?w=500&q=80" },
      { Did: 3, DepartmentName: "Electrical Engineering", ImageUrl: "https://images.unsplash.com/photo-1601597111158-2fceff292cdc?w=500&q=80" },
      { Did: 4, DepartmentName: "Civil Engineering", ImageUrl: "https://images.unsplash.com/photo-1541888086925-920a068018e9?w=500&q=80" }
    ]);
    await db.collection('counters').insertOne({ _id: "did", Seq: 4 });

    // Seed Faculty
    await db.collection('faculties').insertMany([
      {
        fid: 1, FirstName: "Alan", LastName: "Turing", Department: "Computer Science", 
        Username: "alanturing", Password: "password123", Email: "alan@ams.edu",
        Mobile: "9876543210", Gender: "Male", DateOfJoining: "2020-01-15",
        ImageUrl: "https://images.unsplash.com/photo-1472099645785-5658abf4ff4e?w=200&q=80"
      },
      {
        fid: 2, FirstName: "Ada", LastName: "Lovelace", Department: "Computer Science", 
        Username: "adalovelace", Password: "password123", Email: "ada@ams.edu",
        Mobile: "9876543211", Gender: "Female", DateOfJoining: "2019-08-20",
        ImageUrl: "https://images.unsplash.com/photo-1438761681033-6461ffad8d80?w=200&q=80"
      }
    ]);
    await db.collection('counters').insertOne({ _id: "fid", Seq: 2 });

    // Seed Students
    await db.collection('StudentTbl').insertMany([
      {
        Sid: 1, Faculty_Id: "1", Fname: "John", Lname: "Doe", Gender: "Male",
        Email_Id: "johndoe@student.edu", Mo_Number: "9123456780", Department: "Computer Science",
        Password: "password123", ImageUrl: "https://images.unsplash.com/photo-1531427186611-ecfd6d936c79?w=200&q=80",
        DOB: new Date("2000-01-01"), DOA: new Date("2020-08-01"), Address: "123 Main St",
        ParentName: "Richard Doe", ParentMobile: "9000000000", ParentEmail: "richard@example.com"
      }
    ]);
    await db.collection('counters').insertOne({ _id: "sid", Seq: 1 });

    console.log("Database seeded successfully with CORRECT collection names!");
  } finally {
    await client.close();
  }
}

seed().catch(console.error);
