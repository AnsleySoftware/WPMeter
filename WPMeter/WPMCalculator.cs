namespace WPMeter
{
    public class WPMCalculator
    {
        private readonly Queue<DateTime> keyTimestamps = new Queue<DateTime>();
        private readonly TimeSpan trackingWindow = TimeSpan.FromSeconds(10);
        

        private readonly HashSet<Keys> validKeys = new HashSet<Keys>
        {
            Keys.Space,
            Keys.OemPeriod,
            Keys.Oemcomma,
            Keys.OemQuestion,
            Keys.OemSemicolon,
            Keys.OemQuotes,
            Keys.OemOpenBrackets,
            Keys.OemCloseBrackets,
            Keys.OemPipe,
            Keys.OemMinus,
            Keys.Oemplus,
            Keys.Oemtilde
        };

        public WPMCalculator()
        {
            for (Keys k = Keys.A; k <= Keys.Z; k++)
            {
                validKeys.Add(k);
            }

            for (Keys k = Keys.D0; k <= Keys.D9; k++)
            {
                validKeys.Add(k);
            }
        }

        public void HandleKeyPress(Keys key)
        {
            if (!validKeys.Contains(key))
                return;

            Console.WriteLine($"Key Pressed: {key}");


            DateTime now = DateTime.Now;

            while (keyTimestamps.Count > 0 && now - keyTimestamps.Peek() > trackingWindow)
            {
                var removed = keyTimestamps.Dequeue();
                Console.WriteLine($"[Prune] Removed timestamp: {removed:T}");
            }

            keyTimestamps.Enqueue(now);
            Console.WriteLine($"[Add] Added timestamp: {now:T} | Queue size: {keyTimestamps.Count}");
        }


        public double? GetCurrentWpm()
        {
            DateTime now = DateTime.Now;

            // Prune old keys first (important if this is called periodically without new keys)
            while (keyTimestamps.Count > 0 && now - keyTimestamps.Peek() > trackingWindow)
            {
                var removed = keyTimestamps.Dequeue();
                Console.WriteLine($"[Prune@WPM] Removed timestamp: {removed:T}");
            }

            if (keyTimestamps.Count < 2)
                return null;

            TimeSpan effectiveWindow = keyTimestamps.Last() - keyTimestamps.Peek();

            if (effectiveWindow.TotalSeconds < 1)
                return null;

            double words = keyTimestamps.Count / 5.0;
            double minutes = effectiveWindow.TotalMinutes;

            return words / minutes;
        }
    }
}
