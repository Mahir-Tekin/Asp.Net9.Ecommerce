'use client';

export function EditCategoryForm({ categoryId, onCancel }: { categoryId: string; onCancel: () => void }) {
  return (
    <div className="p-4 border rounded bg-white shadow">
      <p>EditCategoryForm for ID: {categoryId} ...</p>
      <div className="mt-4">
        <button onClick={onCancel} className="border px-3 py-1 rounded">Back</button>
      </div>
    </div>
  );
}

export default EditCategoryForm;
