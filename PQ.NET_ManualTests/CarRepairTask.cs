namespace PQ.NET_Examples
{
    internal class CarRepairTask
    {
        public int Id { get; }

        public CarRepairTask(int id)
        {
            this.Id = id;
        }
    }

    internal class NoTask : CarRepairTask
    {
        public NoTask() : base(-1)
        {

        }
    }
}