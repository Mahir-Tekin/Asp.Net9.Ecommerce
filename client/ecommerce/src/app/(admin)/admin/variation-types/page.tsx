'use client'
import { useState } from "react";
import ListVariationTypes from "@/components/admin/variationTypes/ListVariationTypes";
import CreateVariationType from "@/components/admin/variationTypes/CreateVariationType";
import UpdateVariationType from "@/components/admin/variationTypes/UpdateVariationType";
import VariationTypeDetails from "@/components/admin/variationTypes/VariationTypeDetails";

export default function VariationTypesPage() {
    const [view, setView] = useState("list");
    const [selectedId, setSelectedId] = useState<string | null>(null);

    const refreshList = () => {
        // Logic to refresh the list of variation types
    };

    return (
        <div style={{ display: "flex", gap: "1rem" }}>
            {/* Left Column: List */}
            <div style={{ flex: 1 }}>
                <ListVariationTypes
                    onSelect={(id) => {
                        setSelectedId(id);
                        setView("details");
                    }}
                    onCreate={() => setView("create")}
                />
            </div>

            {/* Right Column: Conditional Views */}
            <div style={{ flex: 2 }}>
                {view === "create" && <CreateVariationType onSuccess={refreshList} />}
                {view === "update" && selectedId && (
                    <UpdateVariationType id={selectedId} onSuccess={refreshList} />
                )}
                {view === "details" && selectedId && (
                    <VariationTypeDetails
                        id={selectedId}
                        onEdit={() => setView("update")}
                    />
                )}
            </div>
        </div>
    );
}