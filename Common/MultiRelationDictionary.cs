using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class MultiRelationDictionary<S,T>
    {
        Dictionary<S, List<T>> TByS;
        Dictionary<T, List<S>> SByT;

        public MultiRelationDictionary()
        {
            TByS = new Dictionary<S, List<T>>();
            SByT = new Dictionary<T, List<S>>();
        }

        public void Add(S SValue)
        {
            TByS.Add(SValue, new List<T>());
        }
        public void Add(T TValue)
        {
            SByT.Add(TValue, new List<S>());
        }

        public void AddRelation(S name, T service)
        {
            if (TByS.ContainsKey(name))
            {
                if (!TByS[name].Contains(service))
                {
                    TByS[name].Add(service);
                }
            }
            else
            {
                TByS.Add(name, new List<T> { service });
            }

            if (SByT.ContainsKey(service))
            {
                if (!SByT[service].Contains(name))
                {
                    SByT[service].Add(name);
                }
            }
            else
            {
                SByT.Add(service, new List<S> { name });
            }
        }
        public void AddRelation(T service, S name)
        {
            AddRelation(name, service);
        }

        public void AddRelations(S name, params T[] services)
        {
            if (TByS.ContainsKey(name))
            {
                foreach (var service in services)
                {
                    if (!TByS[name].Contains(service))
                    {
                        TByS[name].Add(service);
                    }
                }
            }
            else
            {
                TByS.Add(name, services.ToList());
            }
            foreach (var service in services)
            {
                if (SByT.ContainsKey(service))
                {
                    if (!SByT[service].Contains(name))
                    {
                        SByT[service].Add(name);
                    }
                }
                else
                {
                    SByT.Add(service, new List<S> { name });
                }
            }
        }
        public void AddRelations(T service, params S[] names)
        {
            if (SByT.ContainsKey(service))
            {
                foreach (var name in names)
                {
                    if (!SByT[service].Contains(name))
                    {
                        SByT[service].Add(name);
                    }
                }
            }
            else
            {
                SByT.Add(service, names.ToList());
            }

            foreach (var name in names)
            {
                if (TByS.ContainsKey(name))
                {
                    if (!TByS[name].Contains(service))
                    {
                        TByS[name].Add(service);
                    }
                }
                else
                {
                    TByS.Add(name, new List<T> { service });
                }
            }
        }

        public void RemoveRelation(S SValue, T TValue)
        {
            if (TByS.ContainsKey(SValue) && TByS[SValue].Contains(TValue))
            {
                TByS[SValue].Remove(TValue);
                if (TByS[SValue].Count == 0)
                {
                    TByS.Remove(SValue);
                }
            }

            if (SByT.ContainsKey(TValue) && SByT[TValue].Contains(SValue))
            {
                SByT[TValue].Remove(SValue);
                if (SByT[TValue].Count == 0)
                {
                    SByT.Remove(TValue);
                }
            }
        }
        public void RemoveRelation(T TValue, S SValue)
        {
            RemoveRelation(SValue, TValue);
        }

        public List<S> this[T service]
        {
            get { return SByT[service]; }
        }
        public List<T> this[S name]
        {
            get { return TByS[name]; }
        }
    }
}
