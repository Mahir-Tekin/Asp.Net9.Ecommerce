interface ProductPriceProps {
  price?: number;
  oldPrice?: number | null;
}

export default function ProductPrice({ price, oldPrice }: ProductPriceProps) {
  if (typeof price !== 'number') {
    return <div className="mb-2 text-gray-400">Please select options to see price</div>;
  }
  const hasDiscount = typeof oldPrice === 'number' && oldPrice > price;
  return (
    <div className="mb-2 flex items-center gap-2">
      <span className="text-xl font-bold text-indigo-700">
        ${price.toLocaleString(undefined, { minimumFractionDigits: 2, maximumFractionDigits: 2 })}
      </span>
      {hasDiscount && (
        <span className="text-base line-through text-gray-500">
          ${oldPrice?.toLocaleString(undefined, { minimumFractionDigits: 2, maximumFractionDigits: 2 })}
        </span>
      )}
    </div>
  );
}
