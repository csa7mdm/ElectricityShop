import { useSelector, useDispatch } from 'react-redux';
import { RootState } from '../store';
import {
  fetchUserOrders,
  fetchAllOrders,
  fetchOrderById,
  createOrder,
  updateOrderStatus,
} from '../store/order/orderSlice';
import { Address, PaginationParams } from '../types';

export const useOrder = () => {
  const dispatch = useDispatch();
  const { orders, order, isLoading, error } = useSelector(
    (state: RootState) => state.order
  );
  
  const handleFetchUserOrders = async () => {
    try {
      await dispatch(fetchUserOrders()).unwrap();
      return true;
    } catch (error) {
      return false;
    }
  };
  
  const handleFetchAllOrders = async (params: PaginationParams) => {
    try {
      await dispatch(fetchAllOrders(params)).unwrap();
      return true;
    } catch (error) {
      return false;
    }
  };
  
  const handleFetchOrderById = async (orderId: string) => {
    try {
      await dispatch(fetchOrderById(orderId)).unwrap();
      return true;
    } catch (error) {
      return false;
    }
  };
  
  const handleCreateOrder = async (
    shippingAddress: Address,
    billingAddress: Address,
    paymentMethod: string
  ) => {
    try {
      await dispatch(
        createOrder({ shippingAddress, billingAddress, paymentMethod })
      ).unwrap();
      return true;
    } catch (error) {
      return false;
    }
  };
  
  const handleUpdateOrderStatus = async (orderId: string, status: string) => {
    try {
      await dispatch(updateOrderStatus({ orderId, status })).unwrap();
      return true;
    } catch (error) {
      return false;
    }
  };
  
  return {
    orders,
    order,
    isLoading,
    error,
    fetchUserOrders: handleFetchUserOrders,
    fetchAllOrders: handleFetchAllOrders,
    fetchOrderById: handleFetchOrderById,
    createOrder: handleCreateOrder,
    updateOrderStatus: handleUpdateOrderStatus,
  };
};
