'use client';

import Image from 'next/image';
import Link from 'next/link';

interface ProductCardProps {
  id: string;
  name: string;
  mainImage: string | null;
  lowestPrice: number;
  lowestOldPrice?: number | null;
}

export default function ProductCard({
  id,
  name,
  mainImage,
  lowestPrice,
  lowestOldPrice,
}: ProductCardProps) {
  const hasDiscount =
    typeof lowestOldPrice === 'number' &&
    lowestOldPrice > 0 &&
    lowestOldPrice > lowestPrice;

  return (
    <div className="bg-white rounded-lg p-4 flex flex-col items-center shadow border">
      {mainImage ? (
        <Image
          src={mainImage}
          alt={name}
          width={180}
          height={180}
          className="object-cover rounded mb-2"
        />
      ) : (
        <div className="w-[180px] h-[180px] bg-gray-200 rounded mb-2 flex items-center justify-center text-gray-400">
          No Image
        </div>
      )}
      <div className="font-semibold text-center mb-1">{name}</div>
      <div className="flex items-center gap-2 mb-2">
        <span className="text-lg font-bold text-indigo-700">
          ${lowestPrice}
        </span>
        {hasDiscount && (
          <span className="text-sm line-through text-gray-500">
            ${lowestOldPrice}
          </span>
        )}
      </div>
      <Link
        href={`/product/${id}`}
        className="mt-auto px-4 py-1 bg-indigo-600 text-white rounded hover:bg-indigo-700 text-sm"
      >
        Go Details
      </Link>
    </div>
  );
}