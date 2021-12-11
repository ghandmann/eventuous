# Reproducer Fork of Eventuous

This repository is just a fork of [Eventuous](https://github.com/Eventuous/eventuous) to provide a simple reproducer for a issue i found.


## How to run

1) Start EventStoreDB with `docker-compose up -d esdb` from the repository root
2) Run `dotnet run` in the `Reproducer` folder
3) After some iterations you should get an exception

## Bug

It looks like that repeating calls to the built in `AggregateStore` trigger a problem with the gRPC based connection to EventStoreDB. This is reproduce by `./Reproducer/Program.cs` with a simple for-loop which just loads the same aggregate over and over again. At the same time the underlying `EventStoreClient` from EventStoreDB is not showing the same behavior.

So far i have seen two exceptions bubbling up:

```
Unhandled exception. Eventuous.ReadFromStreamException: Unable to read events from Booking-20ddda03-ff1e-4474-9899-a80b9658545e: Status(StatusCode="Internal", Detail="Error starting gRPC call. InvalidOperationException: Multiple grpc-status headers.", DebugException="System.InvalidOperationException: Multiple grpc-status headers.
   at Grpc.Net.Client.Internal.GrpcProtocolHelpers.GetHeaderValue(HttpHeaders headers, String name)
   at Grpc.Net.Client.Internal.GrpcProtocolHelpers.TryGetStatusCore(HttpResponseHeaders headers, Nullable`1& status)
   at Grpc.Net.Client.Internal.GrpcCall`2.ValidateHeaders(HttpResponseMessage httpResponse)
   at Grpc.Net.Client.Internal.GrpcCall`2.RunCall(HttpRequestMessage request, Nullable`1 timeout)")
 ---> System.InvalidOperationException: Status(StatusCode="Internal", Detail="Error starting gRPC call. InvalidOperationException: Multiple grpc-status headers.", DebugException="System.InvalidOperationException: Multiple grpc-status headers.
   at Grpc.Net.Client.Internal.GrpcProtocolHelpers.GetHeaderValue(HttpHeaders headers, String name)
   at Grpc.Net.Client.Internal.GrpcProtocolHelpers.TryGetStatusCore(HttpResponseHeaders headers, Nullable`1& status)
   at Grpc.Net.Client.Internal.GrpcCall`2.ValidateHeaders(HttpResponseMessage httpResponse)
   at Grpc.Net.Client.Internal.GrpcCall`2.RunCall(HttpRequestMessage request, Nullable`1 timeout)")
 ---> Grpc.Core.RpcException: Status(StatusCode="Internal", Detail="Error starting gRPC call. InvalidOperationException: Multiple grpc-status headers.", DebugException="System.InvalidOperationException: Multiple grpc-status headers.
   at Grpc.Net.Client.Internal.GrpcProtocolHelpers.GetHeaderValue(HttpHeaders headers, String name)
   at Grpc.Net.Client.Internal.GrpcProtocolHelpers.TryGetStatusCore(HttpResponseHeaders headers, Nullable`1& status)
   at Grpc.Net.Client.Internal.GrpcCall`2.ValidateHeaders(HttpResponseMessage httpResponse)
   at Grpc.Net.Client.Internal.GrpcCall`2.RunCall(HttpRequestMessage request, Nullable`1 timeout)")
   at Grpc.Net.Client.Internal.HttpContentClientStreamReader`2.MoveNextCore(CancellationToken cancellationToken)
   at EventStore.Client.Interceptors.TypedExceptionInterceptor.AsyncStreamReader`1.MoveNext(CancellationToken cancellationToken)
   --- End of inner exception stack trace ---
   at EventStore.Client.Interceptors.TypedExceptionInterceptor.AsyncStreamReader`1.MoveNext(CancellationToken cancellationToken)
   at Grpc.Core.AsyncStreamReaderExtensions.ReadAllAsyncCore[T](IAsyncStreamReader`1 streamReader, CancellationToken cancellationToken)+MoveNext()
   at Grpc.Core.AsyncStreamReaderExtensions.ReadAllAsyncCore[T](IAsyncStreamReader`1 streamReader, CancellationToken cancellationToken)+System.Threading.Tasks.Sources.IValueTaskSource<System.Boolean>.GetResult()
   at EventStore.Client.EventStoreClient.ReadStreamResult.<>c__DisplayClass4_0.<<-ctor>g__GetStateInternal|1>d.MoveNext()
--- End of stack trace from previous location ---
   at EventStore.Client.EventStoreClient.ReadStreamResult.MoveNextAsync()
   at Eventuous.EventStore.EsdbEventStore.<>c__DisplayClass10_0.<<ReadStream>b__0>d.MoveNext() in /home/epplersv/projects/eventuous/src/EventStore/src/Eventuous.EventStore/EsdbEventStore.cs:line 193
--- End of stack trace from previous location ---
   at Eventuous.EventStore.EsdbEventStore.<>c__DisplayClass10_0.<<ReadStream>b__0>d.MoveNext() in /home/epplersv/projects/eventuous/src/EventStore/src/Eventuous.EventStore/EsdbEventStore.cs:line 193
--- End of stack trace from previous location ---
   at Eventuous.EventStore.EsdbEventStore.TryExecute[T](Func`1 func, String stream, Func`1 getError, Func`3 getException) in /home/epplersv/projects/eventuous/src/EventStore/src/Eventuous.EventStore/EsdbEventStore.cs:line 271
   --- End of inner exception stack trace ---
   at Eventuous.EventStore.EsdbEventStore.TryExecute[T](Func`1 func, String stream, Func`1 getError, Func`3 getException) in /home/epplersv/projects/eventuous/src/EventStore/src/Eventuous.EventStore/EsdbEventStore.cs:line 281
   at Eventuous.EventStore.EsdbEventStore.ReadStream(StreamName stream, StreamReadPosition start, Int32 count, Action`1 callback, CancellationToken cancellationToken) in /home/epplersv/projects/eventuous/src/EventStore/src/Eventuous.EventStore/EsdbEventStore.cs:line 189
   at Eventuous.AggregateStore.Load[T](String id, CancellationToken cancellationToken) in /home/epplersv/projects/eventuous/src/Core/src/Eventuous/Store/AggregateStore.cs:line 75
   at Program.<Main>$(String[] args) in /home/epplersv/projects/eventuous/Reproducer/Program.cs:line 42
   at Program.<Main>(String[] args)
   ```

   and

   ```
   Unhandled exception. Eventuous.ReadFromStreamException: Unable to read events from Booking-911603ac-cb10-4a63-ab2b-feb14e5578cf: Status(StatusCode="Internal", Detail="Error starting gRPC call. NullReferenceException: Object reference not set to an instance of an object.", DebugException="System.NullReferenceException: Object reference not set to an instance of an object.
   at System.Net.Http.Headers.HttpHeaders.ReadStoreValues[T](Span`1 values, Object storeValue, HttpHeaderParser parser, Int32& currentIndex)
   at System.Net.Http.Headers.HttpHeaders.GetStoreValuesAsStringOrStringArray(HeaderDescriptor descriptor, Object sourceValues, String& singleValue, String[]& multiValue)
   at System.Net.Http.Headers.HttpHeaders.GetStoreValuesAsStringArray(HeaderDescriptor descriptor, HeaderStoreItemInfo info)
   at System.Net.Http.Headers.HttpHeaders.GetEnumeratorCore()+MoveNext()
   at Grpc.Net.Client.Internal.GrpcProtocolHelpers.BuildMetadata(HttpResponseHeaders responseHeaders)
   at Grpc.Net.Client.Internal.GrpcCall`2.ValidateHeaders(HttpResponseMessage httpResponse)
   at Grpc.Net.Client.Internal.GrpcCall`2.RunCall(HttpRequestMessage request, Nullable`1 timeout)")
 ---> System.InvalidOperationException: Status(StatusCode="Internal", Detail="Error starting gRPC call. NullReferenceException: Object reference not set to an instance of an object.", DebugException="System.NullReferenceException: Object reference not set to an instance of an object.
   at System.Net.Http.Headers.HttpHeaders.ReadStoreValues[T](Span`1 values, Object storeValue, HttpHeaderParser parser, Int32& currentIndex)
   at System.Net.Http.Headers.HttpHeaders.GetStoreValuesAsStringOrStringArray(HeaderDescriptor descriptor, Object sourceValues, String& singleValue, String[]& multiValue)
   at System.Net.Http.Headers.HttpHeaders.GetStoreValuesAsStringArray(HeaderDescriptor descriptor, HeaderStoreItemInfo info)
   at System.Net.Http.Headers.HttpHeaders.GetEnumeratorCore()+MoveNext()
   at Grpc.Net.Client.Internal.GrpcProtocolHelpers.BuildMetadata(HttpResponseHeaders responseHeaders)
   at Grpc.Net.Client.Internal.GrpcCall`2.ValidateHeaders(HttpResponseMessage httpResponse)
   at Grpc.Net.Client.Internal.GrpcCall`2.RunCall(HttpRequestMessage request, Nullable`1 timeout)")
 ---> Grpc.Core.RpcException: Status(StatusCode="Internal", Detail="Error starting gRPC call. NullReferenceException: Object reference not set to an instance of an object.", DebugException="System.NullReferenceException: Object reference not set to an instance of an object.
   at System.Net.Http.Headers.HttpHeaders.ReadStoreValues[T](Span`1 values, Object storeValue, HttpHeaderParser parser, Int32& currentIndex)
   at System.Net.Http.Headers.HttpHeaders.GetStoreValuesAsStringOrStringArray(HeaderDescriptor descriptor, Object sourceValues, String& singleValue, String[]& multiValue)
   at System.Net.Http.Headers.HttpHeaders.GetStoreValuesAsStringArray(HeaderDescriptor descriptor, HeaderStoreItemInfo info)
   at System.Net.Http.Headers.HttpHeaders.GetEnumeratorCore()+MoveNext()
   at Grpc.Net.Client.Internal.GrpcProtocolHelpers.BuildMetadata(HttpResponseHeaders responseHeaders)
   at Grpc.Net.Client.Internal.GrpcCall`2.ValidateHeaders(HttpResponseMessage httpResponse)
   at Grpc.Net.Client.Internal.GrpcCall`2.RunCall(HttpRequestMessage request, Nullable`1 timeout)")
   at Grpc.Net.Client.Internal.HttpContentClientStreamReader`2.MoveNextCore(CancellationToken cancellationToken)
   at EventStore.Client.Interceptors.TypedExceptionInterceptor.AsyncStreamReader`1.MoveNext(CancellationToken cancellationToken)
   --- End of inner exception stack trace ---
   at EventStore.Client.Interceptors.TypedExceptionInterceptor.AsyncStreamReader`1.MoveNext(CancellationToken cancellationToken)
   at Grpc.Core.AsyncStreamReaderExtensions.ReadAllAsyncCore[T](IAsyncStreamReader`1 streamReader, CancellationToken cancellationToken)+MoveNext()
   at Grpc.Core.AsyncStreamReaderExtensions.ReadAllAsyncCore[T](IAsyncStreamReader`1 streamReader, CancellationToken cancellationToken)+System.Threading.Tasks.Sources.IValueTaskSource<System.Boolean>.GetResult()
   at EventStore.Client.EventStoreClient.ReadStreamResult.<>c__DisplayClass4_0.<<-ctor>g__GetStateInternal|1>d.MoveNext()
--- End of stack trace from previous location ---
   at EventStore.Client.EventStoreClient.ReadStreamResult.MoveNextAsync()
   at Eventuous.EventStore.EsdbEventStore.<>c__DisplayClass10_0.<<ReadStream>b__0>d.MoveNext() in /home/epplersv/projects/eventuous/src/EventStore/src/Eventuous.EventStore/EsdbEventStore.cs:line 193
--- End of stack trace from previous location ---
   at Eventuous.EventStore.EsdbEventStore.<>c__DisplayClass10_0.<<ReadStream>b__0>d.MoveNext() in /home/epplersv/projects/eventuous/src/EventStore/src/Eventuous.EventStore/EsdbEventStore.cs:line 193
--- End of stack trace from previous location ---
   at Eventuous.EventStore.EsdbEventStore.TryExecute[T](Func`1 func, String stream, Func`1 getError, Func`3 getException) in /home/epplersv/projects/eventuous/src/EventStore/src/Eventuous.EventStore/EsdbEventStore.cs:line 271
   --- End of inner exception stack trace ---
   at Eventuous.EventStore.EsdbEventStore.TryExecute[T](Func`1 func, String stream, Func`1 getError, Func`3 getException) in /home/epplersv/projects/eventuous/src/EventStore/src/Eventuous.EventStore/EsdbEventStore.cs:line 281
   at Eventuous.EventStore.EsdbEventStore.ReadStream(StreamName stream, StreamReadPosition start, Int32 count, Action`1 callback, CancellationToken cancellationToken) in /home/epplersv/projects/eventuous/src/EventStore/src/Eventuous.EventStore/EsdbEventStore.cs:line 189
   at Eventuous.AggregateStore.Load[T](String id, CancellationToken cancellationToken) in /home/epplersv/projects/eventuous/src/Core/src/Eventuous/Store/AggregateStore.cs:line 75
   at Program.<Main>$(String[] args) in /home/epplersv/projects/eventuous/Reproducer/Program.cs:line 43
   at Program.<Main>(String[] args)
   ```
