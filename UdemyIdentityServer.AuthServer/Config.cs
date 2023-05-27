using IdentityServer4;
using IdentityServer4.Models;
using IdentityServer4.Test;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace UdemyIdentityServer.AuthServer
{
    public static class Config
    {
        public static IEnumerable<ApiResource> GetApiResources()
        {
            return new List<ApiResource>()
            {
                new ApiResource("resource_api1"){
                    Scopes={ "api1.read","api1.write","api1.update" },
                    ApiSecrets = new []{new  Secret("secretapi1".Sha256())
                    }
                },
                new ApiResource("resource_api2")
                {
                       Scopes={ "api2.read","api2.write","api2.update" },
                          ApiSecrets = new []{new  Secret("secretapi2".Sha256()) }
                }
            };
        }

        public static IEnumerable<ApiScope> GetApiScopes()
        {
            return new List<ApiScope>()
            {
                new ApiScope("api1.read", "API 1 için okuma izni"),
                new ApiScope("api1.write", "API 1 için yazma izni"),
                new ApiScope("api1.update", "API 1 için güncelleme izni"),
                new ApiScope("api2.read", "API 2 için okuma izni"),
                new ApiScope("api2.write", "API 2 için yazma izni"),
                new ApiScope("api2.update", "API 2 için güncelleme izni")
            };
        }

        public static IEnumerable<IdentityResource> GetIdentityResources()
        {
            return new List<IdentityResource>()
            {
               //Token icinde mutleq sekilde subject id dediyimiz userin id-si olmalidir ki biz bu tokenin kim ucun istehsal olundugunu bilek.
                new IdentityResources.OpenId(),//subjet id
                new IdentityResources.Profile(), //user haqqinda melumatlari oz icinde olan claim-lerde saxlayirlar
                new IdentityResources.Email(),

                new IdentityResource(){ Name="CountryAndCity", DisplayName="Country and City",Description="Kullanıcının ülke ve şehir bilgisi", UserClaims= new [] {"country","city"}},
                new IdentityResource(){ Name="Roles",DisplayName="Roles", Description="Kullanıcı rolleri", UserClaims=new [] { "role"} },
            };
        }

        public static IEnumerable<TestUser> GetUsers()
        {
            return new List<TestUser>()
            {
                new TestUser
                {
                    SubjectId = "1",
                    Username = "fcakiroglu16",
                    Password = "password",
                    Claims = new List<Claim>()
                    {
                        new Claim("given_name", "Fatih"),
                        new Claim("family_name", "Çakıroğlu"),
                        new Claim("country", "Türkiye"),
                        new Claim("city", "Ankara"),
                        new Claim("role", "admin")
                    }
                },
                new TestUser
                {
                    SubjectId = "3",
                    Username = "omar",
                    Password = "password",
                    Claims = new List<Claim>()
                    {
                        new Claim("given_name", "java_omer"),
                        new Claim("family_name", "Javanshirli"),
                        new Claim("country","Azerbaycan"),
                        new Claim("city","Baki"),
                        new Claim("role","admin")
                    }
                },

                new TestUser
                {
                    SubjectId = "2",
                    Username = "ahmet16",
                    Password = "password",
                    Claims = new List<Claim>()
                    {
                        new Claim("given_name", "Ahmet"),
                        new Claim("family_name", "Çakıroğlu"),
                        new Claim("country", "Türkiye"),
                        new Claim("city", "İstanbul"),
                        new Claim("role", "customer")
                    }
                }
            };
        }

        public static IEnumerable<Client> GetClients()
        {
            return new List<Client>()
            {
                new Client
                {
                    ClientId = "Client1",
                    ClientName = "Client 1 app uygulaması",
                    ClientSecrets = { new Secret("secret".Sha256()) },
                    AllowedGrantTypes = GrantTypes.ClientCredentials,
                    AllowedScopes = { "api1.read" }
                },
                new Client
                {
                    ClientId = "Client2",
                    ClientName = "Client 2 app uygulaması",
                    ClientSecrets = { new Secret("secret".Sha256()) },
                    AllowedGrantTypes = GrantTypes.ClientCredentials,
                    AllowedScopes = { "api1.read", "api1.update", "api2.write", "api2.update" }
                },
                new Client
                {
                    ClientId = "Client1-Mvc",
                    RequirePkce = false,
                    ClientName = "Client 1 app mvc uygulaması",
                    ClientSecrets = { new Secret("secret".Sha256()) },
                    AllowedGrantTypes = GrantTypes.Hybrid,

                    //Bu Uri Token almag prosesini heyata keciren Uri-dir. Xatirlayag biz Client-dan Authorize Endpoint-ne sorgu gonderdyimiz zaman
                    //Bize Authorization Code ve id_token qayidirdi. Hemen bu Authorization Code ile id_token-nin hansi Uri qayidacagini burda bildiririk. 
                    RedirectUris = { "https://localhost:5006/signin-oidc" },

                    //Hem Client1-den cixis hem identityServerden cixis etmeyim ucun bu kod yazilmalidir
                    PostLogoutRedirectUris = { "https://localhost:5006/signout-callback-oidc" },

                    //Elqeli Client-lar Hansi icazelere sahib olacag Scopes-lar bunu bildirir
                    AllowedScopes = {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.Email,
                        "api1.read",
                        IdentityServerConstants.StandardScopes.OfflineAccess,
                        "CountryAndCity",
                        "Roles"
                    },
                    //Access tokenin bitme vaxtinin teyin edilmesi
                    AccessTokenLifetime = 2 * 60 * 60,

                    //Refresh token elde etmek ucun true etmek lazimdir
                    AllowOfflineAccess = true,

                    //Eger Refresh Tokeni yalniz bir defe istifade etmek isdiyirsen se TokenUsage.OneTimeOnly olmalidir
                    //Yox eger Refresh Tokenin bitme vaxtina qeder istifade etmek isdiyirsen se asagida ki kimi olmalidir
                    RefreshTokenUsage = TokenUsage.ReUse,

                    //Refresh Tokenin omru ile bagli bezi ayarlar var. Eger biz TokenExpiration.Sliding bunu secsey bu o demekdir ki,
                    //Eger siz 15 gun icinde bu Refresh Tokene bir sorgu atarsag bu Tokenin omrunu yene 15 gun uzadir.
                    // Yox eger Sabit bir vaxt vermey isdiyirsizse bu zaman asagida ki kimi etmelisiniz.
                    RefreshTokenExpiration = TokenExpiration.Absolute,

                    //Refresh tokene bime vaxtinin verilmesi
                    AbsoluteRefreshTokenLifetime = (int)(DateTime.Now.AddDays(60) - DateTime.Now).TotalSeconds,

                    //login oldugumuza zaman istifadecinin hansi melumatlarinin website-a gonderilmesinin icazelerinin 
                    //istifadeciler terefinnen    customize etmesini isteyirsizse bu true olmalidir.(Onay Sayfasi)
                    //RequireConsent = true
                },
                new Client
                 {
                    ClientId = "Client2-Mvc",
                    RequirePkce = false,
                    ClientName = "Client 2 app mvc uygulaması",
                    ClientSecrets = { new Secret("secret".Sha256()) },
                    AllowedGrantTypes = GrantTypes.Hybrid,
                    RedirectUris = { "https://localhost:5011/signin-oidc" },
                    PostLogoutRedirectUris = { "https://localhost:5011/signout-callback-oidc" },
                    AllowedScopes = {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        "api1.read",
                        "api2.read",
                        IdentityServerConstants.StandardScopes.OfflineAccess,
                        "CountryAndCity",
                        "Roles"
                    },
                    AccessTokenLifetime = 2 * 60 * 60,
                    AllowOfflineAccess = true,
                    RefreshTokenUsage = TokenUsage.ReUse,
                    RefreshTokenExpiration = TokenExpiration.Absolute,
                    AbsoluteRefreshTokenLifetime = (int)(DateTime.Now.AddDays(60) - DateTime.Now).TotalSeconds,
                    RequireConsent = false
                 },
                new Client
                {
                    ClientId="js-client",
                    //Client secret yazmax isdemedik de asagida ki koddan istifade etmek lazimdir.
                    RequireClientSecret=false,
                    ClientName="Js Client (Angular)",
                    AllowedGrantTypes= GrantTypes.Code,//Authorization code grant axis type
                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        "api1.read",
                        "api2.read"
                   
                    },
                    RedirectUris={"http://localhost:4200/callback"},
                    AllowedCorsOrigins={"http://localhost:4200"},
                    PostLogoutRedirectUris={"http://localhost:4200"},
                }
            };
        }
    }
}