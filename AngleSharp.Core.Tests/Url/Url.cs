﻿namespace AngleSharp.Core.Tests
{
    using NUnit.Framework;

    [TestFixture]
    public class UrlTests
    {
        [Test]
        public void ValidityOfValidGoogleHostnameAddress()
        {
            var address = "http://www.google.de";
            var result = new Url(address);
            Assert.IsFalse(result.IsInvalid);
        }

        [Test]
        public void ValidityOfValidGoogleHostnameAndPathEmptyAddress()
        {
            var address = "http://www.google.de/";
            var result = new Url(address);
            Assert.IsFalse(result.IsInvalid);
        }

        [Test]
        public void ValidityOfValidGoogleHostnameAndPathAddress()
        {
            var address = "http://www.google.de/some-path";
            var result = new Url(address);
            Assert.IsFalse(result.IsInvalid);
        }

        [Test]
        public void ValidityOfValidGoogleHostnameAndPathAndFragmentEmptyAddress()
        {
            var address = "http://www.google.de/some-path#";
            var result = new Url(address);
            Assert.IsFalse(result.IsInvalid);
        }

        [Test]
        public void ValidityOfValidGoogleHostnameAndPathAndQueryAndFragmentAddress()
        {
            var address = "http://www.google.de/some-path?a=b#header";
            var result = new Url(address);
            Assert.IsFalse(result.IsInvalid);
        }

        [Test]
        public void ValidityOfInvalidFragmentEmptyHostnameAddress()
        {
            var address = "http://#foo";
            var result = new Url(address);
            Assert.IsTrue(result.IsInvalid);
        }

        [Test]
        public void OriginOfGitHubAddressShouldBeGitHubCom()
        {
            var address = "https://github.com/FlorianRappl/AngleSharp";
            var result = new Url(address);
            Assert.IsFalse(result.IsInvalid);
            Assert.AreEqual("https://github.com", result.Origin);
        }

        [Test]
        public void UrlWithSpecialCharacterDash()
        {
            var address = "http://example-domain.com/image.jpg";
            var result = new Url(address);
            Assert.IsFalse(result.IsInvalid);
            Assert.AreEqual(address, result.Href);
            Assert.AreEqual("example-domain.com", result.HostName);
        }

        [Test]
        public void UrlQueryWithEmDashCharacter()
        {
            var address = "http://test/?hi—there";
            var result = new Url(address);
            Assert.IsFalse(result.IsInvalid);
            Assert.AreEqual("http://test/?hi%E2%80%94there", result.Href);
            Assert.AreEqual("hi%E2%80%94there", result.Query);
        }
    }
}
