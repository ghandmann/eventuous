// Copyright (C) 2021-2022 Ubiquitous AS. All rights reserved
// Licensed under the Apache License, Version 2.0.

using Eventuous.Subscriptions;
using Eventuous.Subscriptions.Filters;

namespace Eventuous.Kafka.Subscriptions; 

public class KafkaBasicSubscription : EventSubscription<KafkaSubscriptionOptions> {
    public KafkaBasicSubscription(KafkaSubscriptionOptions options, ConsumePipe consumePipe) : base(options, consumePipe) { }

    protected override ValueTask Subscribe(CancellationToken cancellationToken) => throw new NotImplementedException();

    protected override ValueTask Unsubscribe(CancellationToken cancellationToken) => throw new NotImplementedException();
}
