"use client";

import React, { useEffect, useState, useRef } from "react";
import { useURLFilters } from "@/hooks/useURLFilters";
import { fetchCategories, type Category } from "@/components/shop/Categories/Categories.api";
import { HiChevronRight, HiChevronDown } from "react-icons/hi2";
import { createPortal } from "react-dom";

export default function CategoryBar() {
  const { filters, updateFilters } = useURLFilters();
  const [categories, setCategories] = useState<Category[]>([]);
  const [loading, setLoading] = useState(true);
  // Track the path of open dropdowns (array of category IDs)
  const [activeDropdownPath, setActiveDropdownPath] = useState<string[]>([]);
  const [dropdownPositions, setDropdownPositions] = useState<{ [key: string]: { top: number; left: number } }>({});
  const dropdownRefs = useRef<{ [key: string]: HTMLDivElement | null }>({});
  const buttonRefs = useRef<{ [key: string]: HTMLButtonElement | null }>({});

  useEffect(() => {
    fetchCategories()
      .then(setCategories)
      .catch((err) => {
        console.error("Failed to fetch categories for homepage:", err);
      })
      .finally(() => setLoading(false));
  }, []);

  // Close all dropdowns when clicking outside
  useEffect(() => {
    const handleClickOutside = (event: MouseEvent) => {
      const dropdownElements = Object.values(dropdownRefs.current).filter(Boolean);
      if (dropdownElements.every(ref => ref && !ref.contains(event.target as Node))) {
        setActiveDropdownPath([]);
      }
    };
    document.addEventListener("mousedown", handleClickOutside);
    return () => document.removeEventListener("mousedown", handleClickOutside);
  }, []);

  const handleCategoryClick = (categoryId: string) => {
    updateFilters({
      searchTerm: "",
      categoryId,
      minPrice: "",
      maxPrice: "",
      variationFilters: {},
      sortBy: "",
    });
    setActiveDropdownPath([]); // Close all dropdowns after selection
  };

  // Open dropdown for a category at a given path
  const handleDropdownOpen = (category: Category, parentPath: string[] = []) => {
    const path = [...parentPath, category.id];
    const button = buttonRefs.current[category.id];
    if (button) {
      const rect = button.getBoundingClientRect();
      setDropdownPositions(prev => ({
        ...prev,
        [category.id]: {
          top: rect.bottom + window.scrollY + 8,
          left: rect.left + window.scrollX + (parentPath.length > 0 ? button.offsetWidth : 0)
        }
      }));
    }
    setActiveDropdownPath(path);
  };

  if (loading) {
    return (
      <div className="bg-white border-b border-gray-200">
        <div className="max-w-7xl mx-auto px-4 py-4">
          <div className="flex space-x-4 overflow-x-auto">
            {[...Array(6)].map((_, i) => (
              <div key={i} className="flex-shrink-0 w-32 h-10 bg-gray-200 rounded-lg animate-pulse" />
            ))}
          </div>
        </div>
      </div>
    );
  }

  if (categories.length === 0) return null;

  // Recursive dropdown renderer
  const renderDropdown = (subCategories: Category[], parentPath: string[] = []) => {
    if (!subCategories || subCategories.length === 0) return null;
    const currentId = parentPath[parentPath.length - 1];
    const isOpen = activeDropdownPath[parentPath.length - 1] === currentId;
    if (!isOpen) return null;

    return subCategories.map((subcategory) => {
      const hasSub = subcategory.subCategories && subcategory.subCategories.length > 0;
      const dropdownPos = dropdownPositions[subcategory.id];
      return (
        <React.Fragment key={subcategory.id}>
          {createPortal(
            <div
              className="fixed bg-white border border-gray-200 rounded-lg shadow-xl min-w-56 max-h-80 overflow-y-auto z-[9999]"
              style={dropdownPos ? { top: dropdownPos.top, left: dropdownPos.left } : {}}
              ref={el => { dropdownRefs.current[subcategory.id] = el; }}
            >
              <div className="py-2">
                {subcategory.subCategories && subcategory.subCategories.length > 0 && (
                  <>
                    {subcategory.subCategories.map(child => (
                      <button
                        key={child.id}
                        ref={el => { buttonRefs.current[child.id] = el; }}
                        onClick={e => {
                          e.stopPropagation();
                          if (child.subCategories && child.subCategories.length > 0) {
                            handleDropdownOpen(child, [...parentPath, subcategory.id]);
                          } else {
                            handleCategoryClick(child.id);
                          }
                        }}
                        className={`w-full text-left px-4 py-3 text-sm flex items-center justify-between transition-colors duration-200 hover:bg-gray-50 ${
                          child.id === filters.categoryId
                            ? "bg-purple-50 text-purple-700 font-medium border-l-4 border-purple-500"
                            : "text-gray-700 hover:text-gray-900"
                        }`}
                      >
                        <span>{child.name}</span>
                        {child.subCategories && child.subCategories.length > 0 && (
                          <HiChevronRight className="ml-2 h-4 w-4" />
                        )}
                      </button>
                    ))}
                  </>
                )}
              </div>
            </div>,
            document.body
          )}
          {/* Recursively render deeper dropdowns */}
          {hasSub && activeDropdownPath.includes(subcategory.id)
            ? renderDropdown(subcategory.subCategories, [...parentPath, subcategory.id])
            : null}
        </React.Fragment>
      );
    });
  };

  return (
    <>
      <div className="bg-white border-b border-gray-200 shadow-sm">
        <div className="max-w-7xl mx-auto px-4 py-4">
          <div className="flex items-center space-x-4">
            {/* Browse All Categories */}
            <button
              onClick={() => handleCategoryClick("")}
              className={`flex-shrink-0 flex items-center space-x-2 px-4 py-2 rounded-lg transition-all duration-200 shadow-md hover:shadow-lg ${
                !filters.categoryId
                  ? "bg-gradient-to-r from-purple-600 to-blue-600 text-white"
                  : "bg-gray-100 hover:bg-gray-200 text-gray-700 hover:text-gray-900"
              }`}
            >
              <span className="font-medium">All Categories</span>
              <HiChevronRight className="h-4 w-4" />
            </button>

            {/* Main Category Buttons - Scrollable container */}
            <div className="flex-1 overflow-x-auto scrollbar-hide">
              <div className="flex items-center space-x-4 min-w-max">
                {categories.map((category) => {
                  const isSelected = category.id === filters.categoryId ||
                    category.subCategories?.some(sub => sub.id === filters.categoryId);
                  const hasSubcategories = category.subCategories && category.subCategories.length > 0;
                  const isDropdownOpen = activeDropdownPath[0] === category.id;
                  return (
                    <div key={category.id} className="flex-shrink-0">
                      <button
                        ref={el => { buttonRefs.current[category.id] = el; }}
                        onClick={e => {
                          e.stopPropagation();
                          if (hasSubcategories) {
                            handleDropdownOpen(category, []);
                          } else {
                            handleCategoryClick(category.id);
                          }
                        }}
                        className={`flex items-center space-x-2 px-4 py-2 rounded-lg transition-all duration-200 font-medium whitespace-nowrap border ${
                          isSelected
                            ? "bg-blue-100 border-blue-300 text-blue-700"
                            : "bg-gray-100 hover:bg-gray-200 text-gray-700 hover:text-gray-900 border-gray-200 hover:border-gray-300"
                        }`}
                      >
                        <span>{category.name}</span>
                        {hasSubcategories && (
                          <HiChevronDown className={`h-4 w-4 transition-transform ${
                            isDropdownOpen ? "rotate-180" : ""
                          }`} />
                        )}
                      </button>
                      {/* Render dropdown for this main category */}
                      {hasSubcategories && isDropdownOpen && renderDropdown([category], [])}
                    </div>
                  );
                })}
              </div>
            </div>
          </div>
        </div>
      </div>
      {/* Render recursive dropdowns using portal */}
      {/* (Handled above in the button map) */}
    </>
  );
}
