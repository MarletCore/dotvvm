import typescript from 'rollup-plugin-typescript'
import resolve from 'rollup-plugin-node-resolve';
import commonjs from 'rollup-plugin-commonjs';
import livereload from 'rollup-plugin-livereload';
import { terser } from 'rollup-plugin-terser'
import replace from '@rollup/plugin-replace';

const build = process.env.BUILD || "debug";
const production = build == "production";
const useTerser = production;

const config = ({minify, input, output}) => ({
  input,

  output: [
    { format: 'esm',
      dir: `./obj/javascript/${output}`,
      sourcemap: !production },
    // { format: 'system',
    //   dir: `./Resources/Scripts/out/${output}-system`,
    //   sourcemap: !production },
  ],

//   treeshake: false,
  plugins: [
    typescript({ target: "es2018" }),
    resolve({ browser: true }),
    commonjs(),
    replace({
      "compileConstants.isSpa": true,
      "compileConstants.nomodules": "false",
    }),

    terser({
      ecma: 6,
      compress: true,
      output: {
        beautify: !minify
      },
      mangle: {
        keep_fnames: true,
        keep_classnames: true,
        reserved: [".*"],
        properties: {
          debug: false,
          regex: "triggerMissedEventsOnSubscribe|_.*"
        }
      }
    }),

    minify && terser({
      ecma: 6,
      // compress: false,
      compress: {
        pure_getters: true,
        unsafe_proto: true,
        unsafe_methods: true,
        unsafe_undefined: true,
        unsafe: true,
        unsafe_arrows: true,
        unsafe_comps: true,
        unsafe_math: true,
        hoist_funs: true,
        hoist_vars: true,
        passes: 2,
        pure_funcs: [ 'Math.floor', "ko.unwrap", "ko.isObservable", "ko.observable", "ko.observableArray", "ko.pureComputed", "Math.floor", "Math.pow", "createArray", "Array.isArray", "Object.keys" ]
      },
      mangle: {
        keep_classnames: !production,
        keep_fnames: !production
      },
      output: {
        ecma: 8,
        indent_level: 4,
        beautify: !production,
        ascii_only: true
      }
    }),
    // !production && livereload('public')
  ]
})

export default [
  // config({ minify: production, input: ['./Resources/Scripts/dotvvm-root.ts', './Resources/Scripts/dotvvm-light.ts'], output: "default" }),
  config({ minify: true, input: ['./Resources/Scripts/dotvvm-root.ts'], output: "root-only" }),
]
