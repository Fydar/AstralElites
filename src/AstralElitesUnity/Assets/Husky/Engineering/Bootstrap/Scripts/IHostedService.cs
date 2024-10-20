namespace HuskyUnity.Engineering.Bootstrap
{
    /// <summary>
    /// <para>A component of an <see cref="IHost"/>.</para>
    /// </summary>
    public interface IHostedService
    {
        /// <summary>
        /// <para>Invoked when the <see cref="IHost"/> this <see cref="IHostedService"/> belongs to is started via <see cref="IHost.StartHost"/>.</para>
        /// </summary>
        /// <param name="host">The <see cref="IHost"/> that this instance of <see cref="IHostedService"/> belongs to.</param>
        public void OnStartHost(
            IHost host);

        /// <summary>
        /// <para>Invoked when the <see cref="IHost"/> this <see cref="IHostedService"/> belongs to is ended via <see cref="IHost.StopHost"/>.</para>
        /// </summary>
        /// <param name="host">The <see cref="IHost"/> that this instance of <see cref="IHostedService"/> belongs to.</param>
        public void OnStopHost(
            IHost host);
    }
}
