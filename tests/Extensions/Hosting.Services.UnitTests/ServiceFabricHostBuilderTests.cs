﻿using System.Fabric;
using System.ServiceModel.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Omex.Extensions.Hosting.Services;
using Microsoft.Omex.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Hosting.Services.UnitTests
{
	[TestClass]
	public class ServiceFabricHostBuilderTests
	{
		[TestMethod]
		public void HostPropagatesTypesRegistration()
		{
			HostBuilder hostBuilder = new HostBuilder();

			new ServiceFabricHostBuilder<ServiceContext>(hostBuilder)
			.ConfigureServices((context, collection) =>
			{
				collection.AddTransient<TestTypeToResolve>();
			});

			TestTypeToResolve obj = hostBuilder.Build().Services.GetService<TestTypeToResolve>();

			Assert.IsNotNull(obj);
		}


		private class TestTypeToResolve
		{
		}
	}


	[TestClass]
	public class ServiceFabricMachineInformationTests
	{
		[TestMethod]
		public void MachineInformationInitialization()
		{
			Mock<IHostEnvironment> enviromentMock = new Mock<IHostEnvironment>();
			Mock<IServiceContextAccessor<StatelessServiceContext>> contextMock = new Mock<IServiceContextAccessor<StatelessServiceContext>>();
			IMachineInformation info = new ServiceFabricMachineInformation(enviromentMock.Object, contextMock.Object);
			Assert.Fail();
		}
	}
}