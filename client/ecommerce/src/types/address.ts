// Address type shared by AddressPanel and AddressCard
export type Address = {
  id: string;
  firstName: string;
  lastName: string;
  phoneNumber: string;
  city: string;
  district: string;
  neighborhood: string;
  addressLine: string;
  addressTitle: string;
  isMain: boolean;
  isDeleted: boolean;
};
