import React from 'react';
import { ShoppingCart } from '.';

interface CartSidebarWrapperProps {
  isCartOpen: boolean;
  closeCart: () => void;
}

const CartSidebarWrapper: React.FC<CartSidebarWrapperProps> = ({ isCartOpen, closeCart }) => {
  return <ShoppingCart isOpen={isCartOpen} onClose={closeCart} />;
};

export default CartSidebarWrapper;
