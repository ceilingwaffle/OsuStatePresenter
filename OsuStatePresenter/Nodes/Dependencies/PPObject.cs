namespace OsuStatePresenter.Nodes.Dependencies
{
    // ReSharper disable InconsistentNaming
    internal class PPObject
    {
        public PPObject(double accPP, double pp, double speedPP, double aimPP, double speedStars, double aimStars)
        {
            this.AccPP = accPP;
            this.PP = pp;
            this.SpeedPP = speedPP;
            this.AimPP = aimPP;
            this.SpeedStars = speedStars;
            this.AimStars = aimStars;
        }

        public double PP { get; }

        public double AccPP { get; }

        public double SpeedPP { get; }

        public double AimPP { get; }

        public double SpeedStars { get; }

        public double AimStars { get; }
    }
}