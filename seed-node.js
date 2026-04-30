const { MongoClient } = require('mongodb');

async function seed() {
  const uri = "mongodb://localhost:27017";
  const client = new MongoClient(uri);

  try {
    await client.connect();
    const db = client.db('AMS');

    // Drop collections
    try { await db.collection('faculties').drop(); } catch (e) {}
    try { await db.collection('StudentTbl').drop(); } catch (e) {}
    try { await db.collection('Departments').drop(); } catch (e) {}
    try { await db.collection('counters').drop(); } catch (e) {}
    try { await db.collection('AttendanceRecords').drop(); } catch (e) {}
    try { await db.collection('Attendance').drop(); } catch (e) {}
    try { await db.collection('History').drop(); } catch (e) {}

    // Seed Departments
    await db.collection('Departments').insertMany([
      { Did: 1, DepartmentName: "Computer Science", ImageUrl: "https://images.unsplash.com/photo-1517694712202-14dd9538aa97?w=500&q=80" },
      { Did: 2, DepartmentName: "Mechanical Engineering", ImageUrl: "https://images.unsplash.com/photo-1581092334651-ddf26d9a09d0?w=500&q=80" },
      { Did: 3, DepartmentName: "Electrical Engineering", ImageUrl: "https://images.unsplash.com/photo-1601597111158-2fceff292cdc?w=500&q=80" },
      { Did: 4, DepartmentName: "Civil Engineering", ImageUrl: "https://images.unsplash.com/photo-1541888086925-920a068018e9?w=500&q=80" }
    ]);
    await db.collection('counters').insertOne({ _id: "did", Seq: 4 });

    // Seed Faculty
    await db.collection('faculties').insertMany([
      {
        fid: 1,
        FirstName: "Alan", LastName: "Turing", Department: "Computer Science", 
        Username: "alanturing", Password: "password123", Email: "alan@ams.edu",
        Mobile: "9876543210", Gender: "Male", DateOfJoining: "2020-01-15",
        ImageUrl: "https://images.unsplash.com/photo-1472099645785-5658abf4ff4e?w=200&q=80"
      },
      {
        fid: 2,
        FirstName: "Ada", LastName: "Lovelace", Department: "Computer Science", 
        Username: "adalovelace", Password: "password123", Email: "ada@ams.edu",
        Mobile: "9876543211", Gender: "Female", DateOfJoining: "2019-08-20",
        ImageUrl: "https://images.unsplash.com/photo-1438761681033-6461ffad8d80?w=200&q=80"
      },
      {
        fid: 3,
        FirstName: "Nikola", LastName: "Tesla", Department: "Electrical Engineering", 
        Username: "ntesla", Password: "password123", Email: "nikola@ams.edu",
        Mobile: "9876543212", Gender: "Male", DateOfJoining: "2015-05-10",
        ImageUrl: "https://images.unsplash.com/photo-1500648767791-00dcc994a43e?w=200&q=80"
      }
    ]);
    await db.collection('counters').insertOne({ _id: "fid", Seq: 3 });

    // Seed Students
    await db.collection('StudentTbl').insertMany([
      {
        Sid: 1, Faculty_Id: "1", Fname: "John", Lname: "Doe", Gender: "Male",
        Email_Id: "johndoe@student.edu", Mo_Number: "9123456780", Department: "Computer Science",
        Password: "password123",
        ImageUrl: "https://images.unsplash.com/photo-1531427186611-ecfd6d936c79?w=200&q=80"
      },
      {
        Sid: 2, Faculty_Id: "2", Fname: "Jane", Lname: "Smith", Gender: "Female",
        Email_Id: "janesmith@student.edu", Mo_Number: "9123456781", Department: "Computer Science",
        Password: "password123",
        ImageUrl: "https://images.unsplash.com/photo-1494790108377-be9c29b29330?w=200&q=80"
      },
      {
        Sid: 3, Faculty_Id: "3", Fname: "Mike", Lname: "Johnson", Gender: "Male",
        Email_Id: "mikej@student.edu", Mo_Number: "9123456782", Department: "Electrical Engineering",
        Password: "password123",
        ImageUrl: "https://images.unsplash.com/photo-1599566150163-29194dcaad36?w=200&q=80"
      }
    ]);
    await db.collection('counters').insertOne({ _id: "sid", Seq: 3 });

    console.log("Database seeded successfully with new fresh data containing ImageUrls!");
  } finally {
    await client.close();
  }
}

seed().catch(console.error);
