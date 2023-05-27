﻿using IdentityModel.Client;
using IdentityServer.Client1.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;

namespace IdentityServer.Client1.Controllers
{
    public class LoginController : Controller
    {
        private readonly IConfiguration _configuration;

        public LoginController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IActionResult Index()
        {
            return View();
        }


        [HttpPost]
        public async Task<IActionResult>Index(LoginViewModel loginViewModel)
        {
            var client = new HttpClient();

            var disco = await client.GetDiscoveryDocumentAsync(_configuration["AuthServerUrl"]);

            if (disco.IsError)
            {
                //hata yakalama ve loglama
            }

            var password = new PasswordTokenRequest();

            password.Address = disco.TokenEndpoint;
            password.UserName = loginViewModel.Email;
            password.Password = loginViewModel.Password;
            password.ClientId = _configuration["ClientResourceOwner:ClientId"];
            password.ClientSecret = _configuration["ClientResourceOwner:ClientSecret"];

            var token = await client.RequestPasswordTokenAsync(password);

            if (token.IsError)
            {
                ModelState.AddModelError("", "Email veya şifreniz yanlış");
                return View();

                //hata yakalama ve loglama
            }

            var userinfoRequest = new UserInfoRequest();

            userinfoRequest.Token = token.AccessToken;
            userinfoRequest.Address = disco.UserInfoEndpoint;
            var userinfo = await client.GetUserInfoAsync(userinfoRequest);

            if (userinfo.IsError)
            {
                //hata yakalama ve loglama
            }

            ClaimsIdentity claimsIdentity = new ClaimsIdentity(userinfo.Claims, CookieAuthenticationDefaults.AuthenticationScheme, "name", "role");

            ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

            var authenticationProperties = new AuthenticationProperties();

            authenticationProperties.StoreTokens(new List<AuthenticationToken>()
            {
                      new AuthenticationToken{ Name=OpenIdConnectParameterNames.AccessToken,Value= token.AccessToken},
                            new AuthenticationToken{ Name=OpenIdConnectParameterNames.RefreshToken,Value= token.RefreshToken},
                                  new AuthenticationToken{ Name=OpenIdConnectParameterNames.ExpiresIn,Value= DateTime.UtcNow.AddSeconds(token.ExpiresIn).ToString("o", CultureInfo.InvariantCulture)}
            });

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal, authenticationProperties);

            return RedirectToAction("Index", "User");
        }

        //[HttpPost]
        //public async Task<IActionResult> Index(LoginViewModel request)
        //{
        //    // Resource Owner Credentials axisi ile sign in kodu yazmag. Bu axis zamani sigin programin oz icinde bas verdiyi ucun
        //    //biz usere bildermeden Identity servere melumat oturub data elde etmeliyiy. Asagidaki kodlarlar bunnu nece etmey olar
        //    // onu codlasdiriram.

        //    //Ilk olarag Identity servenen xeberlse bilmey ucun bir Client-a ehtiyacimiz var.
        //    var client = new HttpClient();

        //    //Arxasindan Discovery Endpointine muraciet edib butun Enpointleri elde edirik.
        //    //this.configuration["AuthServerUrl" AppsettingJson faildan eldi edirin "AuthServerUrl" bunu
        //    var disco = await client.GetDiscoveryDocumentAsync(this.configuration["AuthServerUrl"]);

        //    if (disco.IsError)
        //    {
        //        //hata ve loglama hissesi
        //    }

        //    var password = new PasswordTokenRequest();
        //    password.Address = disco.TokenEndpoint;
        //    password.UserName = request.Email;
        //    password.Password = request.Password;
        //    password.ClientId = configuration["ClientResourceOwner:ClientId"];
        //    password.ClientSecret = configuration["ClientResourceOwner:ClientSecret"];

        //    //Xatirlayaq Buradaki Password Bizim Resource Owner Credentials axis tipini bildirir. Method bize Token gaytarir.
        //    var token = await client.RequestPasswordTokenAsync(password);

        //    if (token.IsError)
        //    {
        //        //hata ve loglama hissesi
        //    }

        //    var userInfoRequest = new UserInfoRequest();

        //    //Xatirlayag UserInfo Endpointinnen Istifadecinin datalarini elde etmek ucun token gondermey lazim idi.
        //    userInfoRequest.Token = token.AccessToken;
        //    userInfoRequest.Address = disco.UserInfoEndpoint;

        //    //Token-i elde etdikden sora UserInfo Endpoint-ne muraciet edib Userin Role melumatlarini, email melumatini ve.s elde edirik
        //    var userInfo = await client.GetUserInfoAsync(userInfoRequest);

        //    if (userInfo.IsError)
        //    {
        //        //hata ve loglama hissesi
        //    }

        //    // ------------------------------- Bura qeder olan hissede neynedik---------------------------------
        //    //Token Elde etdik.
        //    //User melumatlatini elde etdik
        //    //Artig bu User melumatlarinnen bir Claim yarada bilerem.
        //    // ====================================================================================================================//

        //    //ClaimsIdentity =>Userin melumatlarinnan emele gelen bir sexsiyyet vesiqesi. Bu class bizdem IEnumerable Claims isdeyeyir
        //    //ve Schema isdiyir.Calims-lari yuxarida elde etdiyimiz userInfo-dan elde edirik. Schemani ise startup file-da 
        //    // opts.DefaultScheme => buna ne asign etmisikse onu veririk
        //    ClaimsIdentity claimsIdentity = new ClaimsIdentity(userInfo.Claims, CookieAuthenticationDefaults.AuthenticationScheme);

        //    //Create ClaimPrincipial. ClaimPrincipial clasi biz den bir identity isdiyir. Bizim IIdentity interfacini implement 
        //    // elemis bir clasa ehtiyacmiz var.Bu Clasimizda ClaimsIdentity Olacag.
        //    ClaimsPrincipal claimsPrincipa = new ClaimsPrincipal(claimsIdentity);

        //    //===============================Authentican Propertilerin elave edilmesi=================================//
        //    //Access token ,Refresh Token ,id_token

        //    var authenticationProperties = new AuthenticationProperties();
        //    authenticationProperties.StoreTokens(new List<AuthenticationToken>()
        //    {
        //        new AuthenticationToken{ Name=OpenIdConnectParameterNames.IdToken,Value= token.IdentityToken},
        //        new AuthenticationToken{ Name=OpenIdConnectParameterNames.AccessToken,Value= token.AccessToken},
        //        new AuthenticationToken{ Name=OpenIdConnectParameterNames.RefreshToken,Value= token.RefreshToken},
        //        new AuthenticationToken{ Name=OpenIdConnectParameterNames.ExpiresIn,Value=
        //        DateTime.UtcNow.AddSeconds(token.ExpiresIn).ToString("o", CultureInfo.InvariantCulture)}
        //    });

        //    //Sigin Proces
        //    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipa, authenticationProperties);

        //    return RedirectToAction("Index", "User");
        //}
    }
}
