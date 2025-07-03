"use client";

import React, { useEffect, useState, useRef } from "react";
import { useURLFilters } from "@/hooks/useURLFilters";
import { fetchCategories, type Category } from "@/components/shop/Categories/Categories.api";
import { HiChevronRight, HiChevronDown, HiBars3, HiXMark } from "react-icons/hi2";
import { createPortal } from "react-dom";

export default function CategoryBar() {
  const { filters, updateFilters } = useURLFilters();
  const [categories, setCategories] = useState<Category[]>([]);
  const [loading, setLoading] = useState(true);
  const [showMobileMenu, setShowMobileMenu] = useState(false);
  // Track the path of open dropdowns (array of category IDs)
  const [activeDropdownPath, setActiveDropdownPath] = useState<string[]>([]);
  const [dropdownPositions, setDropdownPositions] = useState<{ [key: string]: { top: number; left: number } }>({});
  const dropdownRefs = useRef<{ [key: string]: HTMLDivElement | null }>({});
  const buttonRefs = useRef<{ [key: string]: HTMLButtonElement | null }>({});
  const mobileMenuRef = useRef<HTMLDivElement>(null);

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
      const isMobileMenuClick = mobileMenuRef.current?.contains(event.target as Node);
      
      if (!isMobileMenuClick && dropdownElements.every(ref => ref && !ref.contains(event.target as Node))) {
        setActiveDropdownPath([]);
        setShowMobileMenu(false);
      }
    };
    document.addEventListener("mousedown", handleClickOutside);
    return () => document.removeEventListener("mousedown", handleClickOutside);
  }, []);

  // Close mobile menu on window resize
  useEffect(() => {
    const handleResize = () => {
      if (window.innerWidth >= 768) {
        setShowMobileMenu(false);
      }
    };
    window.addEventListener('resize', handleResize);
    return () => window.removeEventListener('resize', handleResize);
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
    setShowMobileMenu(false); // Close mobile menu after selection
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

  // Mobile Category Item Component
  const MobileCategoryItem: React.FC<{
    category: Category;
    selectedId: string | null;
    onSelect: (id: string) => void;
    level: number;
  }> = ({ category, selectedId, onSelect, level }) => {
    const [isExpanded, setIsExpanded] = useState(false);
    const hasSubcategories = category.subCategories && category.subCategories.length > 0;
    const isSelected = selectedId === category.id;

    return (
      <div className={`${level > 0 ? 'ml-4' : ''}`}>
        <button
          onClick={() => {
            if (hasSubcategories) {
              setIsExpanded(!isExpanded);
            } else {
              onSelect(category.id);
            }
          }}
          className={`w-full flex items-center justify-between px-4 py-3 rounded-lg transition-all duration-200 text-left ${
            isSelected
              ? "bg-blue-100 text-blue-700 font-medium border border-blue-200"
              : "text-gray-700 hover:bg-gray-50 hover:text-gray-900"
          }`}
        >
          <span>{category.name}</span>
          {hasSubcategories && (
            <HiChevronDown className={`h-4 w-4 transition-transform ${
              isExpanded ? "rotate-180" : ""
            }`} />
          )}
        </button>
        
        {hasSubcategories && isExpanded && (
          <div className="mt-2 space-y-1">
            {category.subCategories!.map((subcategory) => (
              <MobileCategoryItem
                key={subcategory.id}
                category={subcategory}
                selectedId={selectedId}
                onSelect={onSelect}
                level={level + 1}
              />
            ))}
          </div>
        )}
      </div>
    );
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
          <div className="flex items-center justify-between">
            {/* Mobile Menu Button */}
            <button
              onClick={() => setShowMobileMenu(!showMobileMenu)}
              className="md:hidden flex items-center gap-2 px-3 py-2 text-gray-700 hover:text-gray-900 hover:bg-gray-50 rounded-lg transition-colors"
            >
              {showMobileMenu ? <HiXMark className="w-5 h-5" /> : <HiBars3 className="w-5 h-5" />}
              <span className="font-medium">Categories</span>
            </button>

            {/* Desktop Categories */}
            <div className="hidden md:flex items-center space-x-4 flex-1">
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

            {/* Mobile Current Category Display */}
            <div className="md:hidden flex-1 text-center">
              <span className="text-sm text-gray-600">
                {filters.categoryId ? 
                  (() => {
                    const findCategoryName = (id: string, cats: Category[]): string => {
                      for (const cat of cats) {
                        if (cat.id === id) return cat.name;
                        if (cat.subCategories) {
                          const found = findCategoryName(id, cat.subCategories);
                          if (found) return found;
                        }
                      }
                      return 'Category';
                    };
                    return findCategoryName(filters.categoryId, categories);
                  })() : 
                  'All Categories'
                }
              </span>
            </div>
          </div>
        </div>
      </div>

      {/* Mobile Menu Overlay */}
      {showMobileMenu && (
        <div 
          className="md:hidden fixed inset-0 bg-black/50 z-40"
          onClick={() => setShowMobileMenu(false)}
        />
      )}

      {/* Mobile Category Menu */}
      {showMobileMenu && (
        <div 
          ref={mobileMenuRef}
          className="md:hidden fixed top-[70px] left-0 right-0 bg-white shadow-xl z-50 max-h-[calc(100vh-70px)] overflow-y-auto border-t border-gray-200"
        >
          <div className="p-4">
            {/* All Categories Button */}
            <button
              onClick={() => handleCategoryClick("")}
              className={`w-full flex items-center justify-between px-4 py-3 rounded-lg transition-all duration-200 mb-3 ${
                !filters.categoryId
                  ? "bg-gradient-to-r from-purple-600 to-blue-600 text-white"
                  : "bg-gray-100 hover:bg-gray-200 text-gray-700"
              }`}
            >
              <span className="font-medium">All Categories</span>
              <HiChevronRight className="h-4 w-4" />
            </button>

            {/* Mobile Category List */}
            <div className="space-y-2">
              {categories.map((category) => (
                <MobileCategoryItem
                  key={category.id}
                  category={category}
                  selectedId={filters.categoryId}
                  onSelect={handleCategoryClick}
                  level={0}
                />
              ))}
            </div>
          </div>
        </div>
      )}

      {/* Desktop Dropdown Portals */}
      {/* These are rendered in the desktop category buttons above */}
    </>
  );
}
