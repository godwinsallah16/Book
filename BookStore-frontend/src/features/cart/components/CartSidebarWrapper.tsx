import React, { useState } from 'react';
import { ShoppingCart } from '.';

const CartSidebarWrapper: React.FC = () => {
  const [isCartOpen, setIsCartOpen] = useState(false);

  const openCart = () => setIsCartOpen(true);
  const closeCart = () => setIsCartOpen(false);

  return (
    <>
      <button style={{position: 'fixed', top: 20, right: 20, zIndex: 2000}} onClick={openCart}>
        Open Cart
      </button>
      <ShoppingCart isOpen={isCartOpen} onClose={closeCart} />
    </>
  );
};

export default CartSidebarWrapper;
