using Microsoft.Azure.Cosmos;
using Polly;
using Polly.CircuitBreaker;
using Polly.Contrib.Simmy;
using Polly.Contrib.Simmy.Outcomes;
using Polly.Timeout;
using Polly.Wrap;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;

namespace CacheStrategyImplementation.ExecutionPolicies
{
    public class ExecutionPolicy
    {
        private static AsyncTimeoutPolicy CreateTimeOutPolicy(int operationTimeoutInSec,TimeoutStrategy timeoutStrategy)
        {

            var overallOperationTimeOutSpan = TimeSpan.FromSeconds(operationTimeoutInSec);
            var asyncPerCallTimeout =
                Policy.TimeoutAsync(overallOperationTimeOutSpan, timeoutStrategy);

            return asyncPerCallTimeout;
        }

        private static AsyncCircuitBreakerPolicy CreateAsynCircuitBreakerPolicy(int circuitBreakerThresholdExceptions, int circuitBreakerOpenDurationInSeconds)
        {
            var asyncBreakerPolicy = Policy
                .Handle<Exception>()
                .CircuitBreakerAsync(
                    exceptionsAllowedBeforeBreaking: circuitBreakerThresholdExceptions,
                    durationOfBreak: TimeSpan.FromSeconds(circuitBreakerOpenDurationInSeconds)
                );

            return asyncBreakerPolicy;

        }

        private static AsyncInjectOutcomePolicy CreateChaosFaultPolicy<T>(T injectedException)where T:Exception
        {            
            var chaosPolicy = MonkeyPolicy.InjectExceptionAsync(with =>
                with.Fault(injectedException)
                    .InjectionRate(1.0)
                    .Enabled()
                );

            return chaosPolicy;
        }

        public static AsyncPolicyWrap CreateExecutionPolicyWrapper()
        {
            // Following example causes the policy to return a bad request HttpResponseMessage with a probability of 5% if enabled
            var fault = new CosmosException("Simulated cosmos exception", HttpStatusCode.ServiceUnavailable, -1, "", 0);
            AsyncInjectOutcomePolicy injectOutcomePolicy = CreateChaosFaultPolicy<CosmosException>(fault);
            AsyncTimeoutPolicy timeoutPolicy = CreateTimeOutPolicy(5, TimeoutStrategy.Pessimistic);
            return Policy.WrapAsync(timeoutPolicy, injectOutcomePolicy);
        }

    }
}
