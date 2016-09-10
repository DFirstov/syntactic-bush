namespace SyntacticBush.Test
{
    internal class Word
    {
        public string Text { get; set; }

        public Word(string text)
        {
            Text = text;
        }

        public override string ToString() => Text;

        private bool Equals(Word other)
        {
            return string.Equals(Text, other.Text);
        }

        public override bool Equals(object obj)
        {
            return !ReferenceEquals(null, obj) &&
                   (ReferenceEquals(this, obj) ||
                    obj.GetType() == GetType() &&
                    Equals((Word)obj));
        }

        public override int GetHashCode()
        {
            // ReSharper disable once NonReadonlyMemberInGetHashCode
            return Text?.GetHashCode() ?? 0;
        }
    }
}
