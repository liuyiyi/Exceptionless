﻿using System;
using System.Collections.Generic;
using Exceptionless.Core.Billing;
using Exceptionless.Core.Models;
using Exceptionless.Core.Extensions;
using Foundatio.Repositories.Utility;
using Foundatio.Utility;

namespace Exceptionless.Tests.Utility {
    internal static class OrganizationData {
        public static IEnumerable<Organization> GenerateOrganizations(BillingManager billingManager, int count = 10, bool generateId = false, string id = null) {
            for (int i = 0; i < count; i++)
                yield return GenerateOrganization(billingManager, generateId, id);
        }

        public static List<Organization> GenerateSampleOrganizations(BillingManager billingManager) {
            return new List<Organization> {
                GenerateSampleOrganization(billingManager),
                GenerateOrganization(billingManager, id: TestConstants.OrganizationId2, inviteEmail: TestConstants.InvitedOrganizationUserEmail),
                GenerateOrganization(billingManager, id: TestConstants.OrganizationId3, inviteEmail: TestConstants.InvitedOrganizationUserEmail),
                GenerateOrganization(billingManager, id: TestConstants.OrganizationId4, inviteEmail: TestConstants.InvitedOrganizationUserEmail),
                GenerateOrganization(billingManager, id: TestConstants.SuspendedOrganizationId, inviteEmail: TestConstants.InvitedOrganizationUserEmail, isSuspended: true),
            };
        }

        public static Organization GenerateSampleOrganization(BillingManager billingManager) {
            return GenerateOrganization(billingManager, id: TestConstants.OrganizationId, name: "Acme", inviteEmail: TestConstants.InvitedOrganizationUserEmail);
        }

        public static Organization GenerateOrganization(BillingManager billingManager, bool generateId = false, string name = null, string id = null, string inviteEmail = null, bool isSuspended = false) {
            var organization = new Organization {
                Id = id.IsNullOrEmpty() ? generateId ? ObjectId.GenerateNewId().ToString() : TestConstants.OrganizationId : id,
                Name = name ?? $"Organization{id}"
            };

            billingManager.ApplyBillingPlan(organization, billingManager.UnlimitedPlan);

            if (!String.IsNullOrEmpty(inviteEmail)) {
                organization.Invites.Add(new Invite {
                    EmailAddress = inviteEmail,
                    Token = Guid.NewGuid().ToString()
                });
            }

            if (isSuspended) {
                organization.IsSuspended = true;
                organization.SuspensionCode = SuspensionCode.Abuse;
                organization.SuspendedByUserId = TestConstants.UserId;
                organization.SuspensionDate = SystemClock.UtcNow;
            }

            return organization;
        }
    }
}