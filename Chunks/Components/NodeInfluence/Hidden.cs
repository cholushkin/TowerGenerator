
namespace TowerGenerator
{
    // Make this node disabled state by default. Some objects don't change their state by groups.
    // They could change the state by 'induction' or 'suppress' or via collisiondependant nodes
    // To hide some objects initially you could use 'Hidden' on the node
    public class Hidden : BaseComponent
    {
    }
}