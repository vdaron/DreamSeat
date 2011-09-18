DreamSeat
========

DreamSeat is a [CouchDB](http://couchdb.apache.org/) client for the .NET plateform. 
It is based on [DreamSeat](https://github.com/soitgoes/DreamSeat) from [Martin Murphy](https://github.com/soitgoes).

DreamSeat is also base on :

 * [Mindtouch Dream](https://github.com/MindTouch/DReAM).
 * [Newtonsoft.Json](http://json.codeplex.com/)

Thanks to Mindtouch Dream, all the API calls can be executed asychronously or sychronously.

Tested compatibility
====================

 * CouchDB 1.0 and 1.1
 * .NET Framework 4.0 or Mono 2.9 (compiled master branch from Nov 20 2010)

DreamSeat Main Features
=======================

 * Complete Synchronous/Asynchronous API
 * Manage Databases, Documents, Attachments, Views, Users, Replication, Change Notifications, ...

DreamSeat usage
==============

## Basics

### Synchronous

    // assumes localhost:5984 and Admin Party if constructor is left blank
    var client = new CouchClient();
    var db = client.GetDatabase("Northwind");
    
    // get document by ID (return a object derived from [JObject](http://james.newtonking.com/projects/json/help/html/T_Newtonsoft_Json_Linq_JObject.htm))
    var doc = GetDocument<JDocument>(string id);

    // get document by ID (strongly typed POCO version)
    var myObj = db.GetDocument<MyObject>("12345");

    // You can also use the asynchronous method signatures ascking to Wait()
    var db2 = client.GetDatabase("Northwind", new Result<CouchDatabase>()).Wait();

### Asynchronous

    // assumes localhost:5984 and Admin Party if constructor is left blank
    var client = new CouchClient();
    client.GetDatabase("Northwind", new Result<CouchDatabase>()).WhenDone(
        a => DatabaseOpened(a),
        e => ProcessException(e)
        );
    }

For more informations and examples of DreamSeat, have a look at the [sample app](https://github.com/vdaron/DreamSeat/tree/master/Samples/ContactManager) code.

