interface ProductTitleProps {
  name: string;
  variantName?: string;
}

export default function ProductTitle({ name, variantName }: ProductTitleProps) {
  return (
    <div className="mb-2">
      <h1 className="text-2xl font-bold mb-1">{name}</h1>
      {variantName && (
        <div className="text-base text-gray-500 mb-2">{variantName}</div>
      )}
    </div>
  );
}
