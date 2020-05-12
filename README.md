# aws-distributed-mutex [![Build Status](https://github.com/Informatievlaanderen/aws-distributed-mutex/workflows/CI/badge.svg)](https://github.com/Informatievlaanderen/aws-distributed-mutex/actions)

A distributed lock (mutex) implementation for AWS using DynamoDB.

## Background

Locking a resource is the typical solution to synchronise parallel processes. The locking can occure for actors in the same process (*critical section* or `lock` statement in C#), across processes on the same machine (machine-wide *mutex* and *semaphore*), across different nodes on the same network (historically implemented as file locking such as Google's Chubby) or completely distributed be it on the same cloud region or across continents - which is our topic here.

In distributed locking, the phrase *lease* is used more often since the locking almost always is associated with a timeout. In distributed computing, all components are deemed to fail or become unavailable at some point hence by having a timout, the risk of locking a resource indefinitely due to the holder going AWOL is eliminated. Safe timeout depends on the scenarios but usually are somewhere between 30 seconds and 5 minutes. If longer duration of lease is needed, it is advised that the holders acquire a shorter lease but renew while they still need the lease longer than original timeout.

The use case is mainly scheduling a process/job across multiple consumers where there could be many consumers yet only one should be allowed to schedule the work at any time.

In Azure, we can use [lease](https://docs.microsoft.com/en-us/rest/api/storageservices/lease-blob) feature of the Blob Storage to build a distributed mutex. Unfortunately Amazon S3 does not provide a similar feature hence the feature must be built using general features of [DynamoDB](https://aws.amazon.com/dynamodb/).

## DistributedLock Usage

`DistributedLock` is a simplified pattern in which you acquire a lock for the duration of your program. Behind the scenes this is done with an auto-renewing lease.

```csharp
public class Example
{
    public void Main()
    {
        var distributedLock = new DistributedLock<Example>(
            new DistributedLockOptions
            {
                Region = RegionEndpoint.EUWest1,
                AwsAccessKeyId = "xxx",
                AwsSecretAccessKey = "xxx",
                TableName = "__DistributedLocks__",
                LeasePeriod = TimeSpan.FromMinutes(5),
                ThrowOnFailedRenew = true,
                TerminateApplicationOnFailedRenew = true
            });

        if (!distributedLock.AcquireLock())
        {
            Console.WriteLine("Could not get lock, another instance is busy");
            return;
        }

        // Do stuff

        distributedLock.ReleaseLock();
    }
}
```

## Mutex Usage

### Create a DynamoDB table

Create a table named `__lock__` (or any other name as long as you pass the name using settings object) with `resourceId` as the key. You may choose to enable TTL for old locks to disappear.

You can also pass a settings object with option to create the table if it does not exist, so this is done in the code.

### Create a Mutex

If you have configured your Access Key and ID in the config or Environment Variables:

``` csharp
var mutex = new DynamoDBMutex(RegionEndpoint.EUWest1);
```

Otherwise create a client using your method of choice and pass it in to the constructor:

``` csharp
var client = ... // create a client
var mutex = new DynamoDBMutex(client);
```

### Lock/Lease a resource

The *resourceId* string can be anything - essentially is application dependent and as for this library, as long as resource IDs have a one to one relationship with the actual resources, it is fine.

``` csharp
var token = await mutex.AcquireLockAsync("myid", TimeSpan.FromMinutes(1));
```

If token is null, then it measn the operation was not successful, otherwise a token is returned. The token can be used to renew or release the lock (lease). If none happens, it will timeout although the physical record will stay. When you create the DynamoDB table, you can opt for TTL to clear up old leases.

### Release or Renew the lease

It is important to release the lock as long as the work is done

``` csharp
await mutex.ReleaseLockAsync(token);
```

For renewing, pass the original token again to renew for the intended duration. If it is successful, you will reveive another token otherwise return value will be null:

``` csharp
var newToken = mutex.RenewAsync(token, TimeSpan.FromMinutes(1));
```
