using SlackNet;

namespace NexusMods.Monitor.Bot.Slack.Application
{
    public sealed class AttachmentFieldBuilder
    {
        private readonly Field _field;

        public AttachmentFieldBuilder()
        {
            _field = new Field();
        }

        public Field Build() => _field;

        public AttachmentFieldBuilder WithName(string name)
        {
            _field.Title = name;
            return this;
        }

        public AttachmentFieldBuilder WithValue(string value)
        {
            _field.Value = value;
            return this;
        }

        public AttachmentFieldBuilder WithIsInline(bool @bool)
        {
            _field.Short = @bool;
            return this;
        }
    }
}