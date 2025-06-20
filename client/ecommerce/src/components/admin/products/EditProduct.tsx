// Placeholder for EditProduct
'use client';

export default function EditProduct({ id, onSuccess, onCancel }: { id: string; onSuccess: () => void; onCancel: () => void }) {
  return (
    <div>
      <h3>Edit Product (placeholder)</h3>
      <div>Product ID: {id}</div>
      <button onClick={onSuccess}>Save</button>
      <button onClick={onCancel}>Cancel</button>
    </div>
  );
}
