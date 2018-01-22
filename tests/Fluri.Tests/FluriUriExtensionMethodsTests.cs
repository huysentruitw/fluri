using System;
using NUnit.Framework;

namespace Fluri.Tests
{
    [TestFixture]
    public class FluriUriExtensionMethodsTests
    {
        [Test]
        public void WithQueryParameter_AbsoluteUriWithSchemeAndServer_ShouldAddFirstQueryParameter()
        {
            var original = new Uri("http://fluri.org:1234");
            Assert.That(original.WithQueryParameter("abc").ToString(), Is.EqualTo("http://fluri.org:1234/?abc"));
            Assert.That(original.WithQueryParameter("abc", "true").ToString(), Is.EqualTo("http://fluri.org:1234/?abc=true"));
        }

        [Test]
        public void WithQueryParameter_AbsoluteUriWithSchemeServerAndFragment_ShouldAddFirstQueryParameter()
        {
            var original = new Uri("http://fluri.org:1234#fragment");
            Assert.That(original.WithQueryParameter("abc").ToString(), Is.EqualTo("http://fluri.org:1234/?abc#fragment"));
            Assert.That(original.WithQueryParameter("abc", "true").ToString(), Is.EqualTo("http://fluri.org:1234/?abc=true#fragment"));
        }

        [Test]
        public void WithQueryParameter_AbsoluteUriWithSchemeServerPathAndFragment_ShouldAddFirstQueryParameter()
        {
            var original = new Uri("http://fluri.org:1234/some/path#fragment");
            Assert.That(original.WithQueryParameter("abc").ToString(), Is.EqualTo("http://fluri.org:1234/some/path?abc#fragment"));
            Assert.That(original.WithQueryParameter("abc", "true").ToString(), Is.EqualTo("http://fluri.org:1234/some/path?abc=true#fragment"));
        }

        [Test]
        public void WithQueryParameter_AbsoluteUriWithEscapedCharactersInQueryString_ShouldKeepEscapedCharacters()
        {
            var original = new Uri("https://fluri.org/my/endpoint?v%20a=1%202%203");
            Assert.That(original.WithQueryParameter("a b").AbsoluteUri, Is.EqualTo("https://fluri.org/my/endpoint?v%20a=1%202%203&a%20b"));
            Assert.That(original.WithQueryParameter("a b", "12 3").AbsoluteUri, Is.EqualTo("https://fluri.org/my/endpoint?v%20a=1%202%203&a%20b=12%203"));
        }

        [Test]
        public void WithQueryParameter_RelativeUri_ShouldAddFirstQueryParameter()
        {
            var original = new Uri("/test/endpoint", UriKind.Relative);
            Assert.That(original.WithQueryParameter("abc").ToString(), Is.EqualTo("/test/endpoint?abc"));
            Assert.That(original.WithQueryParameter("abc", "123").ToString(), Is.EqualTo("/test/endpoint?abc=123"));
        }

        [Test]
        public void WithQueryParameter_RelativeUriWithQuery_ShouldAddSecondParameter()
        {
            var original = new Uri("/test/endpoint?initial=true", UriKind.Relative);
            Assert.That(original.WithQueryParameter("abc").ToString(), Is.EqualTo("/test/endpoint?initial=true&abc"));
            Assert.That(original.WithQueryParameter("abc", "123").ToString(), Is.EqualTo("/test/endpoint?initial=true&abc=123"));
        }

        [Test]
        public void WithQueryParameter_RelativeUriWithQueryAndFragment_ShouldAddSecondParameter()
        {
            var original = new Uri("/test/endpoint?initial=true#frag", UriKind.Relative);
            Assert.That(original.WithQueryParameter("abc").ToString(), Is.EqualTo("/test/endpoint?initial=true&abc#frag"));
            Assert.That(original.WithQueryParameter("abc", "123").ToString(), Is.EqualTo("/test/endpoint?initial=true&abc=123#frag"));
        }

        [Test]
        public void WithQueryParameter_RelativeUriWithoutLeadingSlash_ShouldAddFirstQueryParameterWithoutAddingLeadingSlash()
        {
            var original = new Uri("test/endpoint", UriKind.Relative);
            Assert.That(original.WithQueryParameter("abc", "123").ToString(), Is.EqualTo("test/endpoint?abc=123"));
        }

        [Test]
        public void WithFragment_AbsoluteUriWithoutFragment_ShouldAddFragment()
        {
            var original = new Uri("https://fluri.org:4433/test?initial=true");
            Assert.That(original.WithFragment("Some Fragment").AbsoluteUri, Is.EqualTo("https://fluri.org:4433/test?initial=true#Some%20Fragment"));
        }

        [Test]
        public void WithFragment_AbsoluteUriWithFragment_ShouldReplaceFragment()
        {
            var original = new Uri("https://fluri.org:4433/test?initial=true#myfragment");
            Assert.That(original.WithFragment("Some Fragment").AbsoluteUri, Is.EqualTo("https://fluri.org:4433/test?initial=true#Some%20Fragment"));
        }

        [Test]
        public void WithFragment_RelativeUriWithoutFragment_ShouldAddFragment()
        {
            var original = new Uri("/test?initial=true", UriKind.Relative);
            Assert.That(original.WithFragment("newfragment").ToString(), Is.EqualTo("/test?initial=true#newfragment"));
        }

        [Test]
        public void WithFragment_RelativeUriWithFragment_ShouldReplaceFragment()
        {
            var original = new Uri("/test?initial=true#myfragment", UriKind.Relative);
            Assert.That(original.WithFragment("newfragment").ToString(), Is.EqualTo("/test?initial=true#newfragment"));
        }
    }
}
