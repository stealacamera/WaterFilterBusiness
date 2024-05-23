import PropTypes from 'prop-types';
import * as Yup from 'yup';
import merge from 'lodash/merge';
import { useSnackbar } from 'notistack';
// form
import { useForm, Controller } from 'react-hook-form';
import { yupResolver } from '@hookform/resolvers/yup';
// @mui
import { 
  Box, Stack, Button, Tooltip, TextField,
  IconButton, DialogActions, Select, MenuItem, 
  FormControl, InputLabel } from '@mui/material';
import { LoadingButton, MobileTimePicker } from '@mui/lab';
// redux
import { useDispatch } from '../../../redux/store';
import { createSchedule, updateSchedule, deleteSchedule } from '../../../redux/slices/calendar';
// components
import Iconify from '../../../components/Iconify';
import { FormProvider } from '../../../components/hook-form';

// ----------------------------------------------------------------------

const weekdayValues = [
  'Monday', 'Tuesday', 'Wednesday', 
  'Thursday', 'Friday', 'Saturday', 'Sunday'];

// ----------------------------------------------------------------------

CalendarForm.propTypes = {
  event: PropTypes.object,
  // range: PropTypes.object,
  onCancel: PropTypes.func,
};

export default function CalendarForm({ event, onCancel }) {
  const { enqueueSnackbar } = useSnackbar();

  const dispatch = useDispatch();

  const isCreating = Object.keys(event).length === 0;

  const ScheduleSchema = Yup.object().shape({
    dayOfWeek: Yup.string().required(),
    beginHour: Yup.string().required()
  });

  const methods = useForm({
    resolver: yupResolver(ScheduleSchema),
    defaultValues: {
      dayOfWeek: weekdayValues[0],
      beginHour: new Date()
    }
  });

  const {
    reset,
    watch,
    control,
    handleSubmit,
    formState: { isSubmitting },
  } = methods;

  const onSubmit = async (data) => {
    try {
      const dateString = (new Date(data.beginHour)).toISOString();
      
      const newSchedule = {
        dayOfWeek: data.dayOfWeek,
        beginHour: dateString.substring(12, dateString.length - 5),
      };

      if (event.id) {
        dispatch(updateSchedule(event.id, newSchedule));
        enqueueSnackbar('Update success!');
      } else {
        enqueueSnackbar('Create success!');
        dispatch(createSchedule(newSchedule));
      }
      onCancel();
      reset();
    } catch (error) {
      console.error(error);
    }
  };

  const handleDelete = async () => {
    if (!event.id) return;
    try {
      onCancel();
      dispatch(deleteSchedule(event.id));
      enqueueSnackbar('Delete success!');
    } catch (error) {
      console.error(error);
    }
  };

  return (
    <FormProvider methods={methods} onSubmit={handleSubmit(onSubmit)}>
      <Stack spacing={3} sx={{ p: 3 }}>
        <Controller
          name="dayOfWeek"
          rules={{required: true}}
          control={control}
          render={() => (
            <FormControl>
              <InputLabel id="weekday-label">Weekday</InputLabel>
              <Select 
                defaultValue={weekdayValues[0]}
                labelId="weekday-label"
                label="Weekday">
                {weekdayValues.map(weekday => (
                  <MenuItem 
                    key={weekday}
                    value={weekday}>
                    {weekday}
                  </MenuItem>
                ))}
              </Select>
            </FormControl>
          )}
        />

        <Controller
          name="beginHour"
          control={control}
          render={({ field }) => (
            <MobileTimePicker
              {...field}
              label="Start hour"
              inputFormat="hh:mm a"
              renderInput={(params) => <TextField {...params} fullWidth />}
            />
          )}
        />
      </Stack>

      <DialogActions>
        {!isCreating && (
          <Tooltip title="Delete Schedule">
            <IconButton onClick={handleDelete}>
              <Iconify icon="eva:trash-2-outline" width={20} height={20} />
            </IconButton>
          </Tooltip>
        )}
        <Box sx={{ flexGrow: 1 }} />

        <Button variant="outlined" color="inherit" onClick={onCancel}>
          Cancel
        </Button>

        <LoadingButton type="submit" variant="contained" loading={isSubmitting}>
          Add
        </LoadingButton>
      </DialogActions>
    </FormProvider>
  );
}
