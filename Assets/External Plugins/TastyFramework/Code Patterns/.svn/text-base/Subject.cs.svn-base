using System.Collections.Generic;

namespace Framework
{
    public class Subject<T>
    {
        public delegate void nFunc(T notifyEvent);
        public delegate void AttachFunc( nFunc func );
        public delegate void DetachFunc(nFunc func);

        List<nFunc> m_attachedFunctions = new List<nFunc>();

        public void Attach( nFunc func )
        {
            if ( !m_attachedFunctions.Contains( func ) )
            {
                m_attachedFunctions.Add( func );
            }
        }

        public void Notify( T notifyEvent )
        {
            foreach ( nFunc func in m_attachedFunctions )
            {
                func( notifyEvent );
            }
        }

        public void Detach(nFunc func)
        {
            if (m_attachedFunctions.Contains(func))
            {
                m_attachedFunctions.Remove(func);
            }
        }

    }
}