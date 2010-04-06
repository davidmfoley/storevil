namespace StorEvil.Core
{
    public struct JobResult
    {
        public int Succeeded { get; set; }
        public int Failed { get; set; }
        public int Pending { get; set; }

        public static JobResult operator + (JobResult a, JobResult b)
        {
            return new JobResult
                       {
                           Succeeded = a.Succeeded + b.Succeeded,
                           Pending = a.Pending + b.Pending,
                           Failed = a.Failed + b.Failed,
                       };
        }
    }
}