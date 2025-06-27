interface AddToCartButtonProps {
  inStock: boolean;
  onClick?: () => void;
}

export default function AddToCartButton({ inStock, onClick }: AddToCartButtonProps) {
  return (
    <button
      className="w-full mt-4 px-4 py-2 bg-green-600 text-white rounded hover:bg-green-700 disabled:bg-gray-400"
      type="button"
      disabled={!inStock}
      onClick={onClick}
    >
      {inStock ? 'Add to Cart' : 'Out of Stock'}
    </button>
  );
}
