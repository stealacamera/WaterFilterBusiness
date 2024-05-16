import { useEffect } from 'react';
import { paramCase } from 'change-case';
import { useParams, useLocation } from 'react-router-dom';
// @mui
import { Container } from '@mui/material';
// redux
import { useDispatch, useSelector } from '../../redux/store';
import { getProducts } from '../../redux/slices/product';
// routes
import { PATH_DASHBOARD } from '../../routes/paths';
// hooks
import useSettings from '../../hooks/useSettings';
// components
import Page from '../../components/Page';
import HeaderBreadcrumbs from '../../components/HeaderBreadcrumbs';
import CallNewEditForm from '../../sections/@dashboard/call-history/CallNewEditForm';

// ----------------------------------------------------------------------

export default function CallHistoryCreate() {
  const { themeStretch } = useSettings();
  const dispatch = useDispatch();
  const { pathname } = useLocation();
  const { id } = useParams();
  const { products } = useSelector((state) => state.product);
  const isEdit = pathname.includes('edit');
  const currentProduct = products.find((product) => paramCase(product.id) === id);

  useEffect(() => {
    dispatch(getProducts());
  }, [dispatch]);

  return (
    <Page title="Create a new call">
      <Container maxWidth={themeStretch ? false : 'lg'}>
        <HeaderBreadcrumbs
          heading={!isEdit ? 'Create a new call' : 'Edit call'}
          links={[
            { name: 'Call History', href: PATH_DASHBOARD.calls.list },
            { name: !isEdit ? 'New Call' : 'Edit Call' },
          ]}
        />

        <CallNewEditForm isEdit={isEdit} currentProduct={currentProduct} />
      </Container>
    </Page>
  );
}
