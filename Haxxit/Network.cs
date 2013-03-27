using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using SmartboyDevelopments.Haxxit.Maps;

namespace SmartboyDevelopments.Haxxit
{
    public class Network
    {
        private List<Server> _root_servers;
        private Dictionary<Guid, Server> _all_servers;
        public IEnumerable<Server> RootServers
        {
            get
            {
                return _root_servers;
            }
        }
        public Network()
        {
            _root_servers = new List<Server>();
            _all_servers = new Dictionary<Guid, Server>();
        }
        public void AddRootServer(Server server)
        {
            if (server != null)
            {
                _root_servers.Add(server);
                Stack<Server> server_stack = new Stack<Server>();
                server_stack.Push(server);
                for (Server s = server_stack.Pop(); server_stack.Count > 0; s = server_stack.Pop())
                {
                    foreach(Server child in s.ChildServers)
                    {
                        if (!_all_servers.ContainsKey(child.Id))
                        {
                            _all_servers.Add(child.Id, child);
                            server_stack.Push(child);
                        }
                    }
                }
            } 
        }
        public void AddToParent(Server parent, Server child)
        {
            _all_servers[parent.Id].AddChildServer(child);
            _all_servers.Add(child.Id, child);
        }
        public bool IsInNetwork(Server server)
        {
            return IsInNetwork(server.Id);
        }
        public bool IsInNetwork(Guid id)
        {
            return _all_servers.ContainsKey(id);
        }
        public void UpdateHackedServer(Server server)
        {
            if (IsInNetwork(server))
                foreach (Server child in server.ChildServers)
                    if (!_all_servers.ContainsKey(child.Id))
                        _all_servers.Add(child.Id, child);
        }
        public void UpdateAllHackedServers()
        {
            foreach (KeyValuePair<Guid, Server> pair in _all_servers)
                foreach (Server child in pair.Value.ChildServers)
                    if (!_all_servers.ContainsKey(child.Id))
                        _all_servers.Add(child.Id, child);
        }
    }

    public class Server
    {
        private Map _map;
        private List<Server> _children;
        private Guid _id;
        public Map ServerMap
        {
            get
            {
                return _map;
            }
        }
        public bool Hacked
        {
            get
            {
                return _map.HasBeenHacked;
            }
        }
        public IEnumerable<Server> ChildServers
        {
            get
            {
                foreach (Server c in _children)
                    if (c.Unlocked)
                        yield return c;
            }
        }
        public Guid Id
        {
            get
            {
                return _id;
            }
        }
        private List<Server> _parents;
        public void AddChildServer(Server child)
        {
            if (child != null)
            {
                foreach (Server c in _children)
                    if (c == child)
                        return;
                child.AddParentServer(this);
                _children.Add(child);
            }
        }
        public void AddParentServer(Server parent)
        {
            if (parent != null)
            {
                foreach (Server p in _parents)
                    if (p == parent)
                        return;
                _parents.Add(parent);
            }
        }
        public bool Unlocked
        {
            get
            {
                foreach (Server p in _parents)
                {
                    if (p.Unlocked && p.Hacked)
                    {
                        return true;
                    }
                }
                return false;
            }
            
        }
        public Server(Map map)
        {
            _map = map;
            _children = new List<Server>();
            _id = Guid.NewGuid();
            _parents = new List<Server>();
        }
    }
}
