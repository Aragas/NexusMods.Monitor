using SlackNet;

using System;
using System.Drawing;

namespace NexusMods.Monitor.Bot.Slack.Application
{
    public sealed class AttachmentBuilder
    {
        private readonly Attachment _attachment;

        public AttachmentBuilder()
        {
            _attachment = new Attachment();
        }

        public Attachment Build() => _attachment;

        public AttachmentBuilder WithTitle(string title)
        {
            _attachment.Title = title;
            return this;
        }

        public AttachmentBuilder WithUrl(string url)
        {
            _attachment.TitleLink = url;
            return this;
        }

        public AttachmentBuilder WithDescription(string description)
        {
            _attachment.Text = description;
            return this;
        }

        public AttachmentBuilder WithThumbnailUrl(string url)
        {
            _attachment.ThumbUrl = url;
            return this;
        }

        public AttachmentBuilder WithAuthor(string author, string avatarUrl, string authorUrl)
        {
            _attachment.AuthorName = author;
            _attachment.AuthorLink = authorUrl;
            _attachment.AuthorIcon = avatarUrl;
            return this;
        }

        public AttachmentBuilder WithTimestamp(DateTimeOffset timestamp)
        {
            return this;
        }

        public AttachmentBuilder WithCurrentTimestamp()
        {
            return this;
        }

        public AttachmentBuilder WithColor(Color color)
        {
            _attachment.Color = $"#{color.R:X2}{color.G:X2}{color.B:X2}";
            return this;
        }

        public AttachmentBuilder WithFooter(string text, string imageUrl)
        {
            _attachment.Footer = text;
            _attachment.FooterIcon = imageUrl;
            return this;
        }

        public AttachmentBuilder WithFields(params AttachmentFieldBuilder[] builders)
        {
            foreach (var fieldBuilder in builders)
                _attachment.Fields.Add(fieldBuilder.Build());
            return this;
        }
    }
}