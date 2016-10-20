using System;
using System.Text;

namespace AVLTree
{
    public class AVLTree
    {
        private int key;
        private int balance;    // value is always in {-1,0,1}
        private bool containsKey;  // umoznuje vytvorit "prazdne" vrcholy
        private bool isHead;     // special designation of the first node of the tree, which contains no data and has root as an only (right) child
        private AVLTree leftChild;
        private AVLTree rightChild;
        private AVLTree parent;

        public int Key { get { return key; } }
        public AVLTree LeftChild { get { return leftChild; } }
        public AVLTree RightChild { get { return rightChild; } }
        public AVLTree Parent { get { return parent; } }

		/// <summary>
		/// The only public constructor, creates the "head" node.
		/// </summary>
		public AVLTree()       
        {
            this.key = Int32.MinValue; // arbitrary value, never used
            this.containsKey = true;
            this.isHead = true;
        }

		/// <summary>
		/// Private constructor, used when creating new nodes by the Insert function. The node is created empty.
		/// </summary>
		/// <param name="parent">Reference to the parent of the constructed node.</param>
		private AVLTree(AVLTree parent)    
        { 
            this.key = 0;
            this.containsKey = false; // the node is created empty
            this.parent = parent;
            this.isHead = false;
        }


		/// <summary>
		/// Rotetes the subtree rooted in the given node to the left.
		/// </summary>
		/// <param name="node">The root of the subtree to be rotated.</param>
		static private void RotateLeft(AVLTree node)   
        {
			// the new local "root"
            AVLTree tmp = node.rightChild;

			// reconnect the references
			if (node.parent != null)
            {
				// find out whether the node was the left or the right child of its parent
                if (node.parent.key > node.key)
                    node.parent.leftChild = tmp;        
                else
                    node.parent.rightChild = tmp;
            }
            tmp.parent = node.parent;
            node.rightChild = tmp.leftChild;
            if (tmp.leftChild != null)
                tmp.leftChild.parent = node;   
            tmp.leftChild = node;
            node.parent = tmp;
            
            // update balances of affected nodes (all scenarios - instead of remembering the depths and recalculating)
            if      ((node.balance == 2) && (tmp.balance == 1))        { node.balance = 0; tmp.balance = 0; }
            else if ((node.balance == 2) && (tmp.balance == -1))       { node.balance = 1; tmp.balance = -2; }
            else if ((node.balance == 2) && (tmp.balance == 2))        { node.balance = -1; tmp.balance = 0; }
            else if ((node.balance == 2) && (tmp.balance == 0))        { node.balance = 1; tmp.balance = -1; }
            else if ((node.balance == 1) && (tmp.balance == 1))        { node.balance = -1; tmp.balance = -1; }
            else if ((node.balance == 1) && (tmp.balance == -1))       { node.balance = 0; tmp.balance = -2; }
            else if ((node.balance == 1) && (tmp.balance == 0))        { node.balance = 0; tmp.balance = -1; }
            
        }

		/// <summary>
		/// Rotetes the subtree rooted in the given node to the right.
		/// </summary>
		/// <param name="node">The root of the subtree to be rotated.</param>
		static private void RotateRight(AVLTree node)
        {
			// the new local "root"
			AVLTree tmp = node.leftChild;

			// reconnect the references
			if (node.parent != null)
            {
				// find out whether the node was the left or the right child of its parent
				if (node.parent.key > node.key)
                    node.parent.leftChild = tmp;
                else
                    node.parent.rightChild = tmp;
            } 
            tmp.parent = node.parent;
            node.leftChild = tmp.rightChild;
            if (tmp.rightChild != null)           
                tmp.rightChild.parent = node;
            tmp.rightChild = node;
            node.parent = tmp;

			// update balances of affected nodes (all scenarios - instead of remembering the depths and recalculating)
			if		((node.balance == -2) && (tmp.balance == -1))       { node.balance = 0; tmp.balance = 0; }
            else if ((node.balance == -2) && (tmp.balance == 1))        { node.balance = -1; tmp.balance = 2; }
            else if ((node.balance == -2) && (tmp.balance == -2))       { node.balance = 1; tmp.balance = 0; }
            else if ((node.balance == -2) && (tmp.balance == 0))        { node.balance = -1; tmp.balance = +1; }
            else if ((node.balance == -1) && (tmp.balance == -1))       { node.balance = 1; tmp.balance = 1; }
            else if ((node.balance == -1) && (tmp.balance == 1))        { node.balance = 0; tmp.balance = 2; }
            else if ((node.balance == -1) && (tmp.balance == 0))        { node.balance = 0; tmp.balance = 1; }
            
        }

		/// <summary>
		/// Inserts the given key into the tree.
		/// </summary>
		/// <param name="key"></param>
		/// <returns>True if the depth of the tree increased.</returns>
        public bool Insert(int key)        
        {
			// empty node, simply fill with key
            if (this.containsKey == false)     
            {
                this.key = key;
                this.containsKey = true;
				// depth increased
                return true;                
            }
			// non-empty node, insert to the correct subtree
            else        
            {
                switch (key.CompareTo(this.key))
                {
					// insert to the right subtree
                    case 1:
                        {
							// create right child if nonexistent
							if (rightChild == null) {
								rightChild = new AVLTree(this);  
							}

							// insert the key to the left subtree
							bool depthIncreased = rightChild.Insert(key);

							// check balance
							if (depthIncreased){
								// head does not need to be balanced
                                if (this.isHead == true)
                                    break; 

                                switch (this.balance)
                                {
									// right subtree was deeper, now increased by another level -> rotation necessary
									case 1:  
                                        {
                                            this.balance++;
											
											// single left rotation
											if (this.rightChild.balance == 1) {
												RotateLeft(this);
											}
											
											// double rotation
                                            else if (this.rightChild.balance == -1) 
                                            {
                                                RotateRight(this.rightChild);
                                                RotateLeft(this);
                                            }
											// balance of the right child cannot be 0, the depth would not increase in such case

											// tree has been balanced and the depth change does not propagate any further
											return false; 
                                        }
                                    case 0:
                                        {
											// right child was balanced, now it will be unbalanced by one (still OK) - propagate the information about depth increase to the upper levels
                                            this.balance++; 
                                            return true;
                                        }
                                    case -1:
                                        {
											// right child was unbalanced to the left, now it will be balanced and the depth will stay the same
											this.balance++; 
                                            return false;
                                        }
                                    default:
                                        break;
                                }
                            }
                            break;
                        }
					// insert to the left subtree (mirror situation)
                    case -1:
                        {
							// create right child if nonexistent
							if (leftChild == null) 
                                leftChild = new AVLTree(this);

							// insert the key to the left subtree
							if (leftChild.Insert(key))
                                {
								// head does not need to be balanced
								if (this.isHead == true)
                                        break;
                                switch (this.balance)
                                {
									// left subtree was deeper, now increased by another level -> rotation necessary
									case -1: 
                                        {
                                            this.balance--;
											
											// single right rotation
											if (this.leftChild.balance == -1) {
												RotateRight(this);
											}

											// double rotation
                                            else if (this.leftChild.balance == 1) 
                                            {
                                                RotateLeft(this.leftChild);
                                                RotateRight(this);
                                            }

											// balance of the left child cannot be 0, the depth would not increase in such case

											// tree has been balanced and the depth change does not propagate any further
											return false;
                                        }
                                    case 0:
                                        {
											// left child was balanced, now it will be unbalanced by one (still OK) - propagate the information about depth increase to the upper levels
											this.balance--; 
                                            return true;
                                        }
                                    case 1:
                                        {
											// left child was unbalanced to the right, now it will be balanced and the depth will stay the same
											this.balance--; 
                                            return false;
                                        }
                                    default:
                                        break;
                                }
                                    
                            }
                            break;
                        }
					// key is already in the tree, do nothing
                    case 0:
                            break; 
                    default:
                        break;
                }
            }
            return false;
        }

		/// <summary>
		/// Deletes the given key from the tree, if present.
		/// </summary>
		/// <param name="key"></param>
		/// <returns>True if the depth of the tree decreased.</returns>
		public bool Delete(int key)
        {
            if (this.isHead)
            {
				// empty tree
				if (this.rightChild == null){
					return false;
				}
				// delete recursively from the tree
				else {
					return this.rightChild.Delete(key);
				}
            }

			// recursively try to find and delete the key
            switch (key.CompareTo(this.key))  
            {
				// look in the right subtree
                case 1:
                    {
						// key is not in the tree, do nothing
						if (rightChild == null) {
							break;
						}

						// delete the key from the right subtree
						bool depthDecreased = rightChild.Delete(key);

						// check balance
						if (depthDecreased)
						{
                            switch (this.balance)
                            {
								// the node is being balanced, but the depth decreases -> send the information to the upper levels
                                case 1:
                                    {
                                        balance--;
                                        return true;
                                    }
								// node is being unbalanced to the left, the depth does NOT decrease
                                case 0:
                                    {
                                        balance--;
                                        return false;
                                    }
								// node is already unbalanced to the left, now double unbalanced -> need to rotate
								case -1:
                                    {
                                        balance--;
                                        switch (leftChild.balance)
                                        {
											// rotate right, depth decreases
                                            case -1:
                                                {
                                                    RotateRight(this);      
                                                    return true;        
                                                }
											// rotate right, depth does NOT decrease
											case 0:
                                                {
                                                    RotateRight(this);      
                                                    return false;
                                                }
											// double rotate, depth decreases
                                            case 1:
                                                {
                                                    RotateLeft(this.leftChild);
                                                    RotateRight(this);
                                                    return true;
                                                }
                                            default:
                                                return false;
                                        }
                                    }
                                default:
                                    break;
                                }
                            }
                        break;
                    }
				// look in the right subtree
                case -1:
                    {
						// key is not in the tree, do nothing
						if (leftChild == null) {
							break;
						}

						// delete the key from the left subtree 
						bool depthDecreased = leftChild.Delete(key);

						// check balance
						if (depthDecreased) 
                        {
                                switch (this.balance)
                                {
									// the node is being balanced, but the depth decreases -> send the information to the upper levels
									case -1:
                                        {
                                            balance++;
                                            return true;
                                        }
									// node is being unbalanced to the right, the depth does NOT decrease
									case 0:
                                        {
                                            balance++;
                                            return false;
                                        }
									// node is already unbalanced to the right, now double unbalanced -> need to rotate
									case 1:    
                                        {
                                            balance++;
                                            switch (rightChild.balance)
                                            {
												// rotate left, depth decreases
												case 1:
                                                    {
                                                        RotateLeft(this);
                                                        return true;
                                                    }
												// rotate left, depth does NOT decrease
												case 0:
                                                    {
                                                        RotateLeft(this);
                                                        return false;
                                                    }
												// double rotate, depth decreases
												case -1:
                                                    {
                                                        RotateRight(this.rightChild);
                                                        RotateLeft(this);
                                                        return true;
                                                    }
                                                default:
                                                    return false;
                                            }
                                        }
                                    default:
                                        break;
                                }
                        }
                        break;
                    }

				// we have found the key, now we will delete it
                case 0:         
                    {
						// node is leaf, simply delete
                        if ((this.rightChild == null) && (this.leftChild == null))  
                        {
                            if (this.key < parent.key)
                                parent.leftChild = null;
                            else
                                parent.rightChild = null;
                            return true;
                        }
						// node has only a left child, just reconnect it
                        else if (this.rightChild == null)        
                        {
                            if (this.key < parent.key)
                            {
                                parent.leftChild = this.leftChild;
                                this.leftChild.parent = this.parent;
                            }
                            else
                            {
                                parent.rightChild = this.leftChild;
                                this.leftChild.parent = this.parent;
                            }
                            return true;
                        }
						// node has only a right child, just reconnect it
						else if (this.leftChild == null)         
                        {
                            if (this.key < parent.key)
                            {
                                parent.leftChild = this.rightChild;
                                this.rightChild.parent = this.parent;
                            }
                            else
                            {
                                parent.rightChild = this.rightChild;
                                this.rightChild.parent = this.parent;
                            }
                            return true;
                        }
						// node has both children, replace it with a node from the subtree
                        else                
                        {
							int replacementKey;
							// choose the replacement in the deeper subtree (avoid disbalancing)
							if (this.balance == -1)         
                                replacementKey = leftChild.findMaxKey();    
                            else
                                replacementKey = rightChild.findMinKey();

							// delete the replacement key (it has surely atmost one child)
                            bool depthDecreased = this.Delete(replacementKey);   

							// replace the key in this node by the key of the deleted replacement
                            this.key = replacementKey;               
                            
							// deleting in the subtree might change depth, propagate
                            return depthDecreased;                          
                        }
                    }
                default:
                    break;
            }
            return false;
        }
        
		/// <summary>
		/// Finds the max key in the tree rooted in the given node.
		/// </summary>
		/// <returns>Max key in the current tree.</returns>
        private int findMaxKey()
            {
                if (rightChild == null)
                    return this.Key;
                else
                    return this.rightChild.findMaxKey();
            }

		/// <summary>
		/// Finds the min key in the tree rooted in the given node.
		/// </summary>
		/// <returns>Min key in the current tree.</returns>
		private int findMinKey()
        {
            if (leftChild == null)
                return this.Key;
            else
                return this.leftChild.findMinKey();
        }

		/// <summary>
		/// Pretty-prints the tree to the string. Levels of the tree are in separate columns; children of a node are to the right, right child is first. Works nicely for keys lower than 1000000.
		/// </summary>
		/// <returns>String representation of the tree.</returns>
        public string Print()
        {
            StringBuilder buffer = new StringBuilder();
            AVLTree node;

			// move to the actual root
            if (this.isHead)
                node = this.rightChild;
            else
                node = this;

            if (node == null){
                return "";
            }

            PrintRecursively(node, buffer, 0); 

            buffer.Append('\n');
            return buffer.ToString();
        }

		/// <summary>
		/// Recursively walks the tree and appends keys and balance indicators of individual nodes to the buffer.
		/// </summary>
		/// <param name="node">Current node.</param>
		/// <param name="buffer">The buffer to write to.</param>
		/// <param name="depth">Current depth.</param>
        private void PrintRecursively(AVLTree node, StringBuilder buffer, int depth)
        {
			// represent the balance as +,- or °
			char balanceIndicator;
            switch (node.balance)     
            {
                case 0: { balanceIndicator = '°'; break; }
                case -1: { balanceIndicator = '-'; break; }
                case 1: { balanceIndicator = '+'; break; }
                default: { balanceIndicator = ' '; break; }
            }

			// print key, balance and indent
			buffer.Append(node.key.ToString() + balanceIndicator + "\t");      

			if (node.rightChild != null)
                PrintRecursively(node.rightChild, buffer, depth + 1);
            if (node.leftChild != null)
            {
				// left child is printed under the right child
                buffer.Append("\n");
                buffer.Append('\t', depth + 1); // indent to get under the right child       
                PrintRecursively(node.leftChild, buffer, depth + 1);
            }
        }
       

		/// <summary>
		/// Attempts to find the key in the tree.
		/// </summary>
		/// <param name="key">The key to be found.</param>
		/// <returns>True if the key is in the tree, false otherwise.</returns>
        public bool Find(int key)
        {
			// head of the tree
            if (this.isHead)
            {
                if (this.rightChild == null)
                    return false;
                else
                    return this.rightChild.Find(key);
            }

			// actual node of the tree
            else
            {
                switch (key.CompareTo(this.key))
                {			
                    case 0: return true;            // key was found
					case 1:
                        {
                            if (this.rightChild == null)
                                return false;
                            else
                                return this.rightChild.Find(key);
                        }
                    case -1:
                        {
                            if (this.leftChild == null)
                                return false;
                            else
                                return this.leftChild.Find(key);
                        }
                    default: return false;
                }
            }
            
        }
    }
}
