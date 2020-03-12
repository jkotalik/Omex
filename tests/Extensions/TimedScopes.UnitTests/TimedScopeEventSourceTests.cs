﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Tracing;
using System.Linq;
using Microsoft.Extensions.Hosting.Internal;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Omex.Extensions.Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Omex.Extensions.TimedScopes.UnitTests
{
	[TestClass]
	public class TimedScopeEventSourceTests
	{
		[DataTestMethod]
		[DataRow(EventSourcesEventIds.LogTimedScopeTestContext, true)]
		[DataRow(EventSourcesEventIds.LogTimedScope, false)]
		public void LogTimedScopeEndEvent_CreatesEvent(EventSourcesEventIds eventId, bool isTransaction)
		{
			CustomEventListener listener = new CustomEventListener();
			listener.EnableEvents(TimedScopeEventSource.Instance, EventLevel.Informational);

			string name = "TestName";
			string subType = "TestSubType";
			string metaData = "TestmetaData";

			Activity activity = new Activity(name);
			using (TimedScope scope = new TimedScope(s_logEventSource, activity, TimedScopeResult.Success, null).Start())
			{
				scope.SubType = subType;
				scope.Metadata = metaData;
				activity.SetUserHash("TestUserHash");
				if (isTransaction)
				{
					activity.MarkAsTransaction();
				}
			}

			EventWrittenEventArgs eventInfo = listener.EventsInformation.Single(e => e.EventId == (int)eventId);

			AssertPayload(eventInfo, "name", name);
			AssertPayload(eventInfo, "subType", subType);
			AssertPayload(eventInfo, "metadata", metaData);
		}


		private static readonly TimedScopeEventSender s_logEventSource =
			new TimedScopeEventSender(
				TimedScopeEventSource.Instance,
				new HostingEnvironment { ApplicationName = "TestApp" },
				new NullLogger<TimedScopeEventSender>());


		private class CustomEventListener : EventListener
		{
			public List<EventWrittenEventArgs> EventsInformation { get; } = new List<EventWrittenEventArgs>();

			protected override void OnEventWritten(EventWrittenEventArgs eventData) => EventsInformation.Add(eventData);
		}


		private void AssertPayload<TPayloadType>(EventWrittenEventArgs info, string name, TPayloadType expected)
			where TPayloadType : class
		{
			int index = info.PayloadNames?.IndexOf(name) ?? -1;

			TPayloadType? value = (TPayloadType?)(index < 0 ? null : info.Payload?[index]);

			Assert.AreEqual(expected, value, $"Wrong value for {name}");
		}
	}
}