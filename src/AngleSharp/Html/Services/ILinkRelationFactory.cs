﻿namespace AngleSharp.Html.Services
{
    using AngleSharp.Html.Dom;
    using AngleSharp.Html.LinkRels;
    using System;

    interface ILinkRelationFactory
    {
        BaseLinkRelation Create(HtmlLinkElement link, String rel);
    }
}