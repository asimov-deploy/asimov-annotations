/*******************************************************************************
* Copyright (C) 2015 eBay Inc.
*
* Licensed under the Apache License, Version 2.0 (the "License");
* you may not use this file except in compliance with the License.
* You may obtain a copy of the License at
*
*   http://www.apache.org/licenses/LICENSE-2.0
*
* Unless required by applicable law or agreed to in writing, software
* distributed under the License is distributed on an "AS IS" BASIS,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the License for the specific language governing permissions and
* limitations under the License.
******************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using AsimovDeploy.Annotations.Agent.Framework.Commands;
using AsimovDeploy.Annotations.Agent.Web.Commands;
using RestSharp;

namespace AsimovDeploy.Annotations.Simulator
{
    public class UnitAndMachines
    {
        private readonly Random _random = new Random();
        public string Dashboard { get; set; }
        public string[] UnitName { get; set; }
        public IEnumerable<string> Machines { get; set; }

        public string GetUnitName()
        {
            return UnitName[_random.Next(0, UnitName.Count() - 1)];
        }
    }

    public class DeployCommandGenerator
    {
        private readonly Random _random;

        public DeployCommandGenerator()
        {
            Defaults();
            _random = new Random();
        }

        private void Defaults()
        {
            Dates = new Between<DateTime>(new DateTime(2015, 1, 1), new DateTime(2015, 3, 1));
            DeployPerDay = new Between<int>(2, 10);
            DeployUnitsPerDeploy = new Between<int>(1, 4);
            UnitNames = new List<UnitAndMachines>
            {
                new UnitAndMachines
                {
                    Dashboard = "Monitoring", 
                    UnitName = new[] {"Tradera.AppLogs.2.Elastic"}, 
                    Machines = new[] {"Monitoring"}
                },
                new UnitAndMachines
                {
                    Dashboard = "CustomerSupport", 
                    UnitName = new[] {"Web.CustomerSupport", "Web.Marketing", "Web.FindingConfiguration", "Web.Finance", "Tradera Admin"}, 
                    Machines = new []{"tradera-cs5"}
                },
                new UnitAndMachines
                {
                    Dashboard = "CustomerSupport", 
                    UnitName = new[] { "CustomerSupport.CustomerSupportHandler","UserRegistration.ApplicantRegistration","UserRegistration.ApplicantFinalization","UserRegistration.ApplicantFullValidation","Listing.ChangeItem","Listing.CreateItem","UserRegistration.ApplicantRegistration","UserRegistration.ApplicantFinalization","UserRegistration.ApplicantFullValidation","Listing.ChangeItem","Listing.CreateItem","Listing.EbayChangeItem","Listing.CreateItem.Handler","Listing.CreateShopItem.Handler","Listing.EbayChangeItem.Handler","Listing.ItemChange","Listing.ItemClosing","Draft.DraftPublishedHandler","MemberManagement.ProfileRefinement","Payment.ProcessingService","Payment.QueryService","Payment.Handlers","OrderManagement.PurchaseOrderStatusHandler","OrderManagement.ItemBoughtArchiverHandler","MemberNotifications.Handler","Marketing.QueryService","Marketing.CommandService","Marketing.MarketingHandler","SelfService.CreateErrandHandler","Buying.BuyItem","Buying.BidPlacement","Buying.BuyingDenormalizer","Buying.Finalization","Buying.BidRemoval","FraudDetectionProcess.FraudDetectionHandler","FraudDetectionProcess.QueryService","FraudDetectionProcess.CommandService","Shipping.ShippingHandler","WebApi.BrowseDenormalizer","WebApi.BrowseDenormalizer.LowPrio","WebApi.MemberActivityDenormalizer","MemberManagement.ProcessFeedback","GlobalBilling.BaaSIntegration","GlobalBilling.CreateFees","GlobalBilling.UpdateMembers","Finding.Notifications.SaveSearches.Service","Finding.Notifications.Service" }, 
                    Machines = new []{ "se1-appsrv-01","se1-appsrv-02","se1-appsrv-03","se1-appsrv-04","se1-appsrv-05" }
                },
                new UnitAndMachines
                {
                    Dashboard = "TraderaSite", 
                    UnitName = new[] {"Tradera.NET", "WebApi", "TouchWeb"}, 
                    Machines = new[]{ "se1-webfront-01", "se1-webfront-02", "se1-webfront-03", "se1-webfront-04", "se1-webfront-05", "se1-webfront-06", "se1-webfront-07", "se1-webfront-08", "se1-webfront-09", "se1-webfront-10", "se1-webfront-11", "se1-webfront-12", "se1-webfront-13", "se1-webfront-14", "se1-webfront-15"} 
                }
            };

            Titles = new List<string>
            {
                "Finding deploy",
                "Selling deploy",
                "Payment deploy",
                "Buying deploy"
            };

            Bodies = new List<string>
            {
                "Body 1",
                "Body 2",
                "Body 3",
                "Body 4",
                "Body 5",
                "Body 6",
            };
            Users = new List<string> { "fhe" };
        }

        public Between<DateTime> Dates { get; set; }
        public Between<int> DeployPerDay { get; set; }
        public Between<int> DeployUnitsPerDeploy { get; set; }
        public List<string> Users { get; set; }
        public List<UnitAndMachines> UnitNames { get; set; }
        public List<string> Titles { get; set; }
        public List<string> Bodies { get; set; }

        public IEnumerable<DateTime> DaysToDeploy()
        {
            var current = Dates.From;
            while (current < Dates.To)
            {
                yield return current;
                current = current.AddDays(1);
            }
        }

        public IEnumerable<AsimovCommand> GenerateDeploymentCommands(DateTime startDateTime, string user)
        {
            var correlationId = Guid.NewGuid().ToString();

            yield return CreateDeployStartedCommand(startDateTime, correlationId);

            foreach (var command in GenerateDeployUnitsPerDeploy(correlationId, user))
            {
                yield return command;
            }

            yield return CreateDeployFinishedCommand(startDateTime, correlationId);
        }

        private DeployFinishedCommand CreateDeployFinishedCommand(DateTime startDateTime, string correlationId)
        {
            return new DeployFinishedCommand
                   {
                       correlationId = correlationId,
                       timestamp = GetEndDate(startDateTime)
                   };
        }

        private DeployStartedCommand CreateDeployStartedCommand(DateTime startDateTime, string correlationId)
        {
            return new DeployStartedCommand
                   {
                       correlationId = correlationId,
                       startedBy = GetUser(),
                       timestamp = startDateTime,
                       title = GetTitle(),
                       body = GetBody()
                   };
        }

        private DateTime GetEndDate(DateTime startDateTime)
        {
            return startDateTime.AddMinutes(10);
        }

        private string GetBody()
        {
            return Bodies[_random.Next(0, Bodies.Count)];
        }

        private string GetTitle()
        {
            return Titles[_random.Next(0, Titles.Count)];
        }

        private string GetUser()
        {
            return Users[_random.Next(0, Users.Count)];
        }

        public IEnumerable<DeployCompletedCommand> GenerateDeployUnitsPerDeploy(string correlationId, string user)
        {
            var index = _random.Next(0, UnitNames.Count);
            var dashboard = UnitNames[index];
            var unit = dashboard.GetUnitName();
            return dashboard.Machines.Select(x => new DeployCompletedCommand
            {
                agentName = x,
                branch = "master",
                correlationId = correlationId,
                eventName = "DeployCompleted",
                oldVersion = "1.0.0",
                status = "Verified",
                unitName = unit,
                userId = user.GetHashCode().ToString(),
                userName = user,
                version = "1.0.1"
            });
        }
    }

    class Program
    {

        static void Main(string[] args)
        {
            Random random = new Random();
            var client = new RestClient("http://localhost:9195");
            // Generate deploy from a start date to an end date
            // Random number per day with random number of deploy units per deploy
            var generator = new DeployCommandGenerator();
            foreach (var day in generator.DaysToDeploy())
            {
                var startdate = day.AddHours(random.Next(8, 10));
                var count = random.Next(generator.DeployPerDay.From, generator.DeployPerDay.To);
                for (var i = 0; i < count; i++)
                {
                    startdate = startdate.AddMinutes(random.Next(15, 55));
                    foreach (var command in generator.GenerateDeploymentCommands(startdate, generator.Users.First()))
                    {
                        var request = new RestRequest(Method.POST)
                        {
                            Resource = GetUrl(command),
                            RequestFormat = DataFormat.Json,
                        };
                        request.AddHeader("Content-Type", "application/json; charset=utf-8");
                        request.AddBody(command);

                        var response = client.Execute(request);
                        if (response.ResponseStatus != ResponseStatus.Completed)
                        {
                            Console.WriteLine(response.StatusDescription);
                            Console.WriteLine(response.ErrorMessage);
                            break;
                        }
                        Console.WriteLine("{0}, {1}", command.correlationId, command.GetType().Name);
                    }
                }
            }
            Console.ReadLine();
        }

        private static string GetUrl(AsimovCommand command)
        {
            if (command is DeployStartedCommand)
            {
                return "/deploy/start";
            }
            if (command is DeployCompletedCommand)
            {
                return "/deploy/unit_completed";
            }
            if (command is DeployFinishedCommand)
            {
                return "/deploy/finished";
            }
            if (command is DeployCancelledCommand)
            {
                return "/deploy/cancelled";
            }
            return null;
        }
    }
}
