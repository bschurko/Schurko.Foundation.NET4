# Schurko.Foundation
A collection of helper, utility, extension and service classes.

## Caching
Caching class that can use in memory storage or file storage.
Caching includes a tag system to store and keep track of cache items.

## Crypto
Classes to encrypt data or objects. Additionally there are classes to manage certificates and RSA encryption.

## Dapper Service
A dapper helper class for executing stored procedures and raw SQL code.
Plenty of extensions that wrap the core dapper library into one easy service.

## Process Manager
A process manager class for executing applications and tools.

## Hash Util
Utility class for hashing data.

## Settings and Configuration
Simplifies interaction with AppSettings.json and AppSetting.config.

## File IO Utility Classes
Utility classes that extend the current .NET file/IO classes for both directories and files.

## Array and CVS Utility
Helper classes where you can parse CVS files and manage them.
Additionally there is an array utility class for extending array functionality.

## CryptoManager 
A simple class to encrypt and decrypt data using AES encryption.

## Heartbeat Writer and Retry Algorithm
A heartbeat diagnostic utility for writing a timestamp at regular intervals to a heartbeat file.
Additionally, the retry algorithm can be used to throttle retry attempts using Fibonacci as the backend algorithm.

## Object Dumper
Takes any object and strips it down to a basic string message that can be used for logs or diagnostics.

## Extended Interlocked
Simple class for thread safe ATOMIC increment and deincrement values.

## Enviroment Util
A class for retriving basic information about a computers CPU and drives.

## String Extensions and Utils
A powerful class full of extensions and utilities for the string object.

## Logger
A simple class for creating and retriving the .NET core Logger class.

## Network Util and Sockets wrapper class
A powerful number of classes that simplify the creation of a socket based server and a socket client for communicating with a socket server.
Additionally, a Network Utility class for getting basic network information.

## Design Patterns and Wrapper classes
Various design pattern wrapper classes. For example, the IRepository/Repository base class allow you to inherit the base class and override the basic UnitOfWork/Repository methods.
The factory class can be used by classes that implement a factory pattern. In addition, a couple more design pattern classes are available to help promote good coding.
Design Pattern Classes: Composite, Factory, Repository NodeT, Singleton.

## Thread-Safe ATOMIC Queue, internal processor and backend Repository
A couple of classes and interfaces that provide a Thread-Safe Queue data structure that allows a custom backend repository.
In addition, an internal processor wrapper class allows jobs that are submitted to the queue to invoke custom Actions/Events when being dequeued or enqueued.

## Hardware Info Utility
A simple class that utilizes WMI to return information about a computers hardware components.

## URL Builder Utility
A simple class for building functional URLs.

## XML Serializers and Utility
A utility class for serializing objects and deserializing them.
Additionally, the XMLUtil class helps with interaction with an XML file.

## Producer and Consumer Design Pattern
An Administrator and worker/IJob class that provides a thread-safe ATOMIC design pattern for submitting jobs that get processed in a parallel fashion.
Simply inherit from the Administrator.cs and override the a few methods for submitting and processing jobs, that get passed into the queue.

## Microsoft Identity Framework Wrapper Classes
Wrapper classes and extensions for the core classes used in the MS Identity Framework.
The IdentityUser class has a abstract AppUserBase that gets implemented by AppUser.cs.
An implementation of the core UserStore.cs using the IdentityDbContext.
Basic extensions for claims and identity base classes.

## Security Impersonation and Credential Provider
Wrapper class for simply applying impersonation to allow security levels to be raised by whatever user provided that you implement.

## .NET Core Dependency Injection Wrapper Class
Simple access to the IServiceCollection class via an Action delegate; allowing you to register classes into the dependency container.
In addition, you can retrieve registered dependencies via the GetService<T> container method.

## Managed Extensibility Framework (MEF) Dependency Injection
n the Managed Extensibility Framework (MEF), a programming model is a particular method of defining the set of conceptual objects on which MEF operates. These conceptual objects include parts, imports, and exports. MEF uses these objects, but does not specify how they should be represented
 
## Message Queue Service
A message queue service that uses a backend repository of a MS SQL tables and stored procedures.
The message queue service includes a simple interface to InMessageQueueAsync and DeMessageQueueAsync messages.
These messages as stored as basic data segments in the database, so an unlimited amount of data can be stored and retrived.

## Event Aggregator
An Event Aggregator acts as a single source of events for many objects. 
You register by subscribing an object event, which gets triggered when the event aggregator broadcasts to all subscriptions.
It registers for all the events of the many objects allowing clients to register with just the aggregator.

## Rabbit MQ Service
A simple wrapper class for Publishing jobs to a RabbitMQ Application Service, as well as Consuming jons that are stored in RabbitMQ.

## Redis Service
A simple wrapper class that provides an easy way to "Set Objects and Strings" to the Redis App Service, as well as "Getting Objects and Strings".

## Socket Server and Client Classes
A set of classes for creating a Socket based Server, as well as a Socket Client for interacting with a Socket Server endpoint.

## Scheduler and Job Processor
A thread-safe parallel scheduler for submitting jobs and executing customized code.

## Tons of Object Extension Methods
Tons of extension methods for tons of classes.
