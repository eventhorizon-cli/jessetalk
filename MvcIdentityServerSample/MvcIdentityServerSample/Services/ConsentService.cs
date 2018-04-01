using IdentityServer4.Models;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using MvcIdentityServerSample.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MvcIdentityServerSample.Services
{
    public class ConsentService
    {
        private readonly IClientStore _clientStore;

        private readonly IResourceStore _resourceStore;

        private readonly IIdentityServerInteractionService _identityServerInteractionService;

        public ConsentService(
            IClientStore clientStore,
            IResourceStore resourceStore,
            IIdentityServerInteractionService identityServerInteractionService)
        {
            _clientStore = clientStore;
            _resourceStore = resourceStore;
            _identityServerInteractionService = identityServerInteractionService;
        }

        public async Task<ConsentViewModel> BuildConsentViewModelAsync(string returnUrl, InputConsentViewModel model = null)
        {
            var request = await _identityServerInteractionService.GetAuthorizationContextAsync(returnUrl);
            if (request == null)
            {
                return null;
            }

            var client = await _clientStore.FindEnabledClientByIdAsync(request.ClientId);
            var resources = await _resourceStore.FindEnabledResourcesByScopeAsync(request.ScopesRequested);

            var vm = CreateConsentViewModel(request, client, resources, model);
            vm.ReturnUrl = returnUrl;
            return vm;
        }

        public async Task<ProcessConsentResult> ProcessGrantAsync(InputConsentViewModel viewModel)
        {
            ConsentResponse consentResponse = null;
            var result = new ProcessConsentResult();
            if (viewModel.Button == "no")
            {
                consentResponse = ConsentResponse.Denied;
            }
            else if (viewModel.Button == "yes")
            {
                if (viewModel.ScopesConsented?.Any() == true)
                {
                    consentResponse = new ConsentResponse
                    {
                        RememberConsent = viewModel.RememberConsent,
                        ScopesConsented = viewModel.ScopesConsented
                    };
                }
                else
                {
                    result.ValidationError = "请至少选中一个权限";
                }
            }

            if (consentResponse != null)
            {
                var request = await _identityServerInteractionService.GetAuthorizationContextAsync(viewModel.ReturnUrl);
                await _identityServerInteractionService.GrantConsentAsync(request, consentResponse);

                result.RedirectUrl = request.RedirectUri;
            }
            else
            {
                var consentViewModel = await BuildConsentViewModelAsync(viewModel.ReturnUrl, viewModel);
                result.ViewModel = consentViewModel;
            }

            return result;
        }

        private ConsentViewModel CreateConsentViewModel(AuthorizationRequest request, Client client, Resources resources, InputConsentViewModel model)
        {
            var selectedScopes = model?.ScopesConsented ?? Enumerable.Empty<string>();
            return new ConsentViewModel
            {
                ClientName = client.ClientName,
                ClientLogoUri = client.LogoUri,
                ClienUri = client.ClientUri,
                RememberConsent = model?.RememberConsent ?? true,

                IdentityScopes = resources.IdentityResources.Select(x => CreateScopeViewModel(x, selectedScopes.Contains(x.Name) || model == null)),
                ResourcesScopes = resources.ApiResources.SelectMany(resource => resource.Scopes.Select(x => CreateScopeViewModel(x, selectedScopes.Contains(x.Name) || model == null)))
            };
        }


        private ScopeViewModel CreateScopeViewModel(IdentityResource identityResource, bool check)
        {
            return new ScopeViewModel
            {
                Name = identityResource.Name,
                DisplayName = identityResource.DisplayName,
                Description = identityResource.Description,
                Required = identityResource.Required,
                Checked = check || identityResource.Required,
            };
        }

        private ScopeViewModel CreateScopeViewModel(Scope scope, bool check)
        {
            return new ScopeViewModel
            {
                Name = scope.Name,
                DisplayName = scope.DisplayName,
                Description = scope.Description,
                Required = scope.Required,
                Checked = check || scope.Required,
                Emphasize = scope.Emphasize
            };
        }
    }
}
